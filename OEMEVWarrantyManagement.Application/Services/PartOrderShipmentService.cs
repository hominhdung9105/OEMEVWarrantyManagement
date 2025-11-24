using Microsoft.AspNetCore.Http;
using OEMEVWarrantyManagement.Application.Dtos;
using OEMEVWarrantyManagement.Application.IRepository;
using OEMEVWarrantyManagement.Application.IServices;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Constants;
using OEMEVWarrantyManagement.Share.Enums;
using OEMEVWarrantyManagement.Share.Exceptions;
using OEMEVWarrantyManagement.Share.Models.Response;
using OfficeOpenXml;

namespace OEMEVWarrantyManagement.Application.Services
{
    public class PartOrderShipmentService : IPartOrderShipmentService
    {
        private readonly IPartOrderRepository _partOrderRepository;
        private readonly IPartOrderItemRepository _partOrderItemRepository;
        private readonly IPartOrderShipmentRepository _shipmentRepository;
        private readonly IPartOrderReceiptRepository _receiptRepository;
        private readonly IPartOrderDiscrepancyResolutionRepository _discrepancyResolutionRepository;
        private readonly IVehiclePartHistoryRepository _vehiclePartHistoryRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPartRepository _partRepository;
        private readonly IPartOrderImageService _imageService;

        public PartOrderShipmentService(
            IPartOrderRepository partOrderRepository,
            IPartOrderItemRepository partOrderItemRepository,
            IPartOrderShipmentRepository shipmentRepository,
            IPartOrderReceiptRepository receiptRepository,
            IPartOrderDiscrepancyResolutionRepository discrepancyResolutionRepository,
            IVehiclePartHistoryRepository vehiclePartHistoryRepository,
            ICurrentUserService currentUserService,
            IPartRepository partRepository,
            IPartOrderImageService imageService)
        {
            _partOrderRepository = partOrderRepository;
            _partOrderItemRepository = partOrderItemRepository;
            _shipmentRepository = shipmentRepository;
            _receiptRepository = receiptRepository;
            _discrepancyResolutionRepository = discrepancyResolutionRepository;
            _vehiclePartHistoryRepository = vehiclePartHistoryRepository;
            _currentUserService = currentUserService;
            _partRepository = partRepository;
            _imageService = imageService;
        }

        public async Task<ShipmentValidationResultDto> ValidateShipmentFileAsync(Guid orderId, IFormFile file)
        {
            var result = new ShipmentValidationResultDto { IsValid = true };

            // Validate order exists and is in Confirm status
            var order = await _partOrderRepository.GetPartOrderByIdAsync(orderId);
            if (order == null)
                throw new ApiException(ResponseError.InvalidOrderId);

            if (order.Status != PartOrderStatus.Confirm.GetPartOrderStatus())
                throw new ApiException(ResponseError.OrderNotConfirmed);

            // Check if shipment already uploaded
            var hasShipments = await _shipmentRepository.HasShipmentsForOrderAsync(orderId);
            if (hasShipments)
                throw new ApiException(ResponseError.ShipmentAlreadyUploaded);

            // Get order items
            var orderItems = await _partOrderItemRepository.GetAllByOrderIdAsync(orderId);
            var requestedQuantities = orderItems.GroupBy(i => i.Model)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            // Parse Excel file
            List<ShipmentExcelRowDto> excelRows;
            try
            {
                excelRows = await ParseShipmentExcelAsync(file);
            }
            catch (Exception)
            {
                throw new ApiException(ResponseError.InvalidExcelFormat);
            }

            if (excelRows.Count == 0)
            {
                result.IsValid = false;
                result.Errors.Add("File Excel không có d? li?u");
                return result;
            }

            // Group by model
            var providedQuantities = excelRows.GroupBy(r => r.Model)
                .ToDictionary(g => g.Key, g => g.Count());

            // Get OEM org ID
            var orgId = await _currentUserService.GetOrgId();

            // Check quantity discrepancies
            foreach (var kvp in requestedQuantities)
            {
                var model = kvp.Key;
                var requested = kvp.Value;
                var provided = providedQuantities.GetValueOrDefault(model, 0);
                var difference = provided - requested;

                var discrepancy = new QuantityDiscrepancyDto
                {
                    Model = model,
                    Requested = requested,
                    Provided = provided,
                    Difference = difference,
                    Status = difference == 0 ? "Match" : (difference > 0 ? "Excess" : "Shortage")
                };

                if (difference != 0)
                {
                    result.IsValid = false;
                    result.QuantityDiscrepancies[model] = discrepancy;
                }
            }

            // Check for extra models not in order
            foreach (var model in providedQuantities.Keys)
            {
                if (!requestedQuantities.ContainsKey(model))
                {
                    result.IsValid = false;
                    result.QuantityDiscrepancies[model] = new QuantityDiscrepancyDto
                    {
                        Model = model,
                        Requested = 0,
                        Provided = providedQuantities[model],
                        Difference = providedQuantities[model],
                        Status = "Excess"
                    };
                }
            }

            // Validate serial numbers
            var serialNumbersSeen = new HashSet<string>();
            foreach (var row in excelRows)
            {
                // Check duplicate
                if (serialNumbersSeen.Contains(row.SerialNumber))
                {
                    result.IsValid = false;
                    result.SerialErrors.Add(new SerialErrorDto
                    {
                        Model = row.Model,
                        SerialNumber = row.SerialNumber,
                        ErrorType = "Duplicate",
                        Message = $"Serial {row.SerialNumber} b? trùng l?p trong file"
                    });
                    continue;
                }
                serialNumbersSeen.Add(row.SerialNumber);

                // Check if serial exists in stock with correct model
                var partHistory = await _vehiclePartHistoryRepository.GetBySerialNumberAsync(row.SerialNumber);
                if (partHistory == null)
                {
                    result.IsValid = false;
                    result.SerialErrors.Add(new SerialErrorDto
                    {
                        Model = row.Model,
                        SerialNumber = row.SerialNumber,
                        ErrorType = "NotInStock",
                        Message = $"Serial {row.SerialNumber} không t?n t?i trong kho"
                    });
                    continue;
                }

                // Check status is InStock
                if (partHistory.Status != VehiclePartCurrentStatus.InStock.GetCurrentStatus())
                {
                    result.IsValid = false;
                    result.SerialErrors.Add(new SerialErrorDto
                    {
                        Model = row.Model,
                        SerialNumber = row.SerialNumber,
                        ErrorType = "NotInStock",
                        Message = $"Serial {row.SerialNumber} không ? tr?ng thái InStock (hi?n t?i: {partHistory.Status})"
                    });
                    continue;
                }

                // Check correct OrgId
                if (partHistory.ServiceCenterId != orgId)
                {
                    result.IsValid = false;
                    result.SerialErrors.Add(new SerialErrorDto
                    {
                        Model = row.Model,
                        SerialNumber = row.SerialNumber,
                        ErrorType = "NotInStock",
                        Message = $"Serial {row.SerialNumber} không thu?c kho c?a t? ch?c này"
                    });
                    continue;
                }

                // Check model matches
                if (partHistory.Model != row.Model)
                {
                    result.IsValid = false;
                    result.SerialErrors.Add(new SerialErrorDto
                    {
                        Model = row.Model,
                        SerialNumber = row.SerialNumber,
                        ErrorType = "WrongModel",
                        Message = $"Serial {row.SerialNumber} thu?c model {partHistory.Model}, không ph?i {row.Model}"
                    });
                    continue;
                }

                // Check if already shipped (in any other order)
                var existingShipment = await _shipmentRepository.GetBySerialNumberAsync(row.SerialNumber);
                if (existingShipment != null)
                {
                    result.IsValid = false;
                    result.SerialErrors.Add(new SerialErrorDto
                    {
                        Model = row.Model,
                        SerialNumber = row.SerialNumber,
                        ErrorType = "AlreadyShipped",
                        Message = $"Serial {row.SerialNumber} ?ã ???c g?i trong ??n hàng khác"
                    });
                }
            }

            // If valid, save shipments temporarily (not yet confirmed)
            if (result.IsValid)
            {
                var shipments = excelRows.Select(r => new PartOrderShipment
                {
                    ShipmentId = Guid.NewGuid(),
                    OrderId = orderId,
                    Model = r.Model,
                    SerialNumber = r.SerialNumber,
                    ShippedAt = DateTime.UtcNow,
                    Status = "Pending" // Will be confirmed later
                }).ToList();

                await _shipmentRepository.AddRangeAsync(shipments);
            }

            return result;
        }

        public async Task ConfirmShipmentAsync(Guid orderId)
        {
            // Validate order exists
            var order = await _partOrderRepository.GetPartOrderByIdAsync(orderId);
            if (order == null)
                throw new ApiException(ResponseError.InvalidOrderId);

            if (order.Status != PartOrderStatus.Confirm.GetPartOrderStatus())
                throw new ApiException(ResponseError.OrderNotConfirmed);

            // Check if shipments exist
            var shipments = await _shipmentRepository.GetByOrderIdAsync(orderId);
            if (!shipments.Any())
                throw new ApiException(ResponseError.ShipmentNotValidated);

            // Update shipment status to Confirmed
            var shipmentList = shipments.ToList();
            foreach (var shipment in shipmentList)
            {
                shipment.Status = "Confirmed";
            }

            // Update order status to InTransit
            order.Status = PartOrderStatus.InTransit.GetPartOrderStatus();
            order.ShippedDate = DateTime.UtcNow;
            await _partOrderRepository.UpdateAsync(order);

            // Update Part stock (decrease EVM stock)
            var orgId = await _currentUserService.GetOrgId();
            var groupedByModel = shipmentList.GroupBy(s => s.Model)
                .ToDictionary(g => g.Key, g => g.Count());

            foreach (var kvp in groupedByModel)
            {
                var part = await _partRepository.GetPartsAsync(kvp.Key, orgId);
                if (part != null)
                {
                    part.StockQuantity -= kvp.Value;
                }
            }

            await _partRepository.UpdateRangeAsync(
                await Task.WhenAll(groupedByModel.Keys.Select(m => _partRepository.GetPartsAsync(m, orgId)))
            );
        }

        public async Task<ReceiptValidationResultDto> ValidateReceiptFileAsync(Guid orderId, IFormFile file)
        {
            var result = new ReceiptValidationResultDto { IsValid = true };

            // Validate order exists and is in InTransit status
            var order = await _partOrderRepository.GetPartOrderByIdAsync(orderId);
            if (order == null)
                throw new ApiException(ResponseError.InvalidOrderId);

            if (order.Status != PartOrderStatus.InTransit.GetPartOrderStatus())
                throw new ApiException(ResponseError.OrderNotInTransit);

            // Get shipments
            var shipments = await _shipmentRepository.GetByOrderIdAsync(orderId);
            if (!shipments.Any())
                throw new ApiException(ResponseError.ShipmentNotValidated);

            var shippedSerials = shipments.ToDictionary(s => s.SerialNumber, s => s);

            // Parse Excel file
            List<ReceiptExcelRowDto> excelRows;
            try
            {
                excelRows = await ParseReceiptExcelAsync(file);
            }
            catch (Exception)
            {
                throw new ApiException(ResponseError.InvalidExcelFormat);
            }

            if (excelRows.Count == 0)
            {
                result.IsValid = false;
                result.Errors.Add("File Excel không có d? li?u");
                return result;
            }

            // Group by model
            var receivedQuantities = excelRows.GroupBy(r => r.Model)
                .ToDictionary(g => g.Key, g => g.Count());

            var shippedQuantities = shipments.GroupBy(s => s.Model)
                .ToDictionary(g => g.Key, g => g.Count());

            // Check quantity discrepancies
            foreach (var kvp in shippedQuantities)
            {
                var model = kvp.Key;
                var shipped = kvp.Value;
                var received = receivedQuantities.GetValueOrDefault(model, 0);
                var difference = received - shipped;

                var discrepancy = new QuantityDiscrepancyDto
                {
                    Model = model,
                    Requested = shipped,
                    Provided = received,
                    Difference = difference,
                    Status = difference == 0 ? "Match" : (difference > 0 ? "Excess" : "Shortage")
                };

                if (difference != 0)
                {
                    result.IsValid = false;
                    result.QuantityDiscrepancies[model] = discrepancy;
                }
            }

            // Check for extra models not shipped
            foreach (var model in receivedQuantities.Keys)
            {
                if (!shippedQuantities.ContainsKey(model))
                {
                    result.IsValid = false;
                    result.QuantityDiscrepancies[model] = new QuantityDiscrepancyDto
                    {
                        Model = model,
                        Requested = 0,
                        Provided = receivedQuantities[model],
                        Difference = receivedQuantities[model],
                        Status = "Excess"
                    };
                }
            }

            // Validate serial numbers
            var serialNumbersSeen = new HashSet<string>();
            foreach (var row in excelRows)
            {
                // Check duplicate
                if (serialNumbersSeen.Contains(row.SerialNumber))
                {
                    result.IsValid = false;
                    result.SerialMismatches.Add(new SerialMismatchDto
                    {
                        Model = row.Model,
                        SerialNumber = row.SerialNumber,
                        ErrorType = "Duplicate",
                        Message = $"Serial {row.SerialNumber} b? trùng l?p trong file"
                    });
                    continue;
                }
                serialNumbersSeen.Add(row.SerialNumber);

                // Check if serial was shipped
                if (!shippedSerials.ContainsKey(row.SerialNumber))
                {
                    result.IsValid = false;
                    result.SerialMismatches.Add(new SerialMismatchDto
                    {
                        Model = row.Model,
                        SerialNumber = row.SerialNumber,
                        ErrorType = "NotShipped",
                        Message = $"Serial {row.SerialNumber} không có trong danh sách ?ã g?i"
                    });
                    continue;
                }

                // Check model matches
                var shippedItem = shippedSerials[row.SerialNumber];
                if (shippedItem.Model != row.Model)
                {
                    result.IsValid = false;
                    result.SerialMismatches.Add(new SerialMismatchDto
                    {
                        Model = row.Model,
                        SerialNumber = row.SerialNumber,
                        ErrorType = "WrongModel",
                        Message = $"Serial {row.SerialNumber} ???c g?i v?i model {shippedItem.Model}, không ph?i {row.Model}"
                    });
                }
            }

            // If valid, save receipts temporarily
            if (result.IsValid)
            {
                var receipts = excelRows.Select(r => new PartOrderReceipt
                {
                    ReceiptId = Guid.NewGuid(),
                    OrderId = orderId,
                    Model = r.Model,
                    SerialNumber = r.SerialNumber,
                    ReceivedAt = DateTime.UtcNow,
                    Status = "Received" // Good condition by default
                }).ToList();

                await _receiptRepository.AddRangeAsync(receipts);
            }

            return result;
        }

        public async Task ConfirmReceiptAsync(Guid orderId, string? damagedPartsJson, List<IFormFile>? images)
        {
            // Validate order exists
            var order = await _partOrderRepository.GetPartOrderByIdAsync(orderId);
            if (order == null)
                throw new ApiException(ResponseError.InvalidOrderId);

            if (order.Status != PartOrderStatus.InTransit.GetPartOrderStatus())
                throw new ApiException(ResponseError.OrderNotInTransit);

            // Check if receipts exist
            var receipts = (await _receiptRepository.GetByOrderIdAsync(orderId)).ToList();
            if (!receipts.Any())
                throw new ApiException(ResponseError.ReceiptNotValidated);

            // Parse damaged parts if provided
            List<DamagedPartInfoDto>? damagedPartInfos = null;
            if (!string.IsNullOrWhiteSpace(damagedPartsJson))
            {
                try
                {
                    damagedPartInfos = System.Text.Json.JsonSerializer.Deserialize<List<DamagedPartInfoDto>>(
                        damagedPartsJson,
                        new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
                    );
                }
                catch (Exception)
                {
                    throw new ApiException(ResponseError.InvalidJsonFormat);
                }
            }

            // Upload images and map to damaged parts
            if (damagedPartInfos != null && damagedPartInfos.Any())
            {
                if (images == null || !images.Any())
                {
                    throw new ApiException(ResponseError.InvalidImage);
                }

                // Validate: s? l??ng ?nh ph?i b?ng s? l??ng damaged parts
                if (images.Count != damagedPartInfos.Count)
                {
                    throw new ApiException(ResponseError.InvalidImage);
                }

                // Upload t?ng ?nh và gán URL
                for (int i = 0; i < damagedPartInfos.Count; i++)
                {
                    var damagedPart = damagedPartInfos[i];
                    var image = images[i];

                    // Upload image
                    var imageUrl = await _imageService.UploadDamagedPartImageAsync(image, orderId, damagedPart.SerialNumber);

                    // Update receipt status
                    var receipt = receipts.FirstOrDefault(r => r.SerialNumber == damagedPart.SerialNumber);
                    if (receipt != null)
                    {
                        receipt.Status = "Damaged";
                        receipt.Note = damagedPart.Note;
                        receipt.ImageUrl = imageUrl;
                    }
                }

                await _receiptRepository.UpdateRangeAsync(receipts);
            }

            // Get SC org ID
            var scOrgId = order.ServiceCenterId;

            // Update VehiclePartHistory for received items (excluding damaged, missing, extra)
            var shipments = await _shipmentRepository.GetByOrderIdAsync(orderId);
            var shippedSerials = shipments.Select(s => s.SerialNumber).ToHashSet();
            var receivedGoodSerials = receipts
                .Where(r => r.Status == "Received")
                .Select(r => r.SerialNumber)
                .ToHashSet();

            // Good serials: shipped AND received with good status
            var goodSerials = shippedSerials.Intersect(receivedGoodSerials).ToList();

            foreach (var serial in goodSerials)
            {
                var partHistory = await _vehiclePartHistoryRepository.GetBySerialNumberAsync(serial);
                if (partHistory != null)
                {
                    partHistory.ServiceCenterId = scOrgId;
                    partHistory.Status = VehiclePartCurrentStatus.InStock.GetCurrentStatus();
                }
            }

            // Update SC Part stock (increase for good parts only)
            var goodReceipts = receipts.Where(r => r.Status == "Received").ToList();
            var groupedByModel = goodReceipts.GroupBy(r => r.Model)
                .ToDictionary(g => g.Key, g => g.Count());

            var partsToUpdate = new List<Part>();
            foreach (var kvp in groupedByModel)
            {
                var part = await _partRepository.GetPartsAsync(kvp.Key, scOrgId);
                if (part != null)
                {
                    part.StockQuantity += kvp.Value;
                    partsToUpdate.Add(part);
                }
            }

            if (partsToUpdate.Any())
            {
                await _partRepository.UpdateRangeAsync(partsToUpdate);
            }

            // Check if there are discrepancies
            var hasDamagedParts = receipts.Any(r => r.Status == "Damaged");
            var hasMissingParts = receipts.Count < shipments.Count();
            var hasExtraParts = receipts.Count > shipments.Count();

            if (hasDamagedParts || hasMissingParts || hasExtraParts)
            {
                // Create discrepancy resolution record
                var resolution = new PartOrderDiscrepancyResolution
                {
                    ResolutionId = Guid.NewGuid(),
                    OrderId = orderId,
                    Status = DiscrepancyResolutionStatus.PendingResolution.GetStatus(),
                    CreatedAt = DateTime.UtcNow
                };

                await _discrepancyResolutionRepository.CreateAsync(resolution);

                // Mark status as DiscrepancyReview
                order.Status = PartOrderStatus.DiscrepancyReview.GetPartOrderStatus();
            }
            else
            {
                // No discrepancies, mark as Done
                order.Status = PartOrderStatus.Done.GetPartOrderStatus();
            }

            order.PartDelivery = DateTime.UtcNow;
            await _partOrderRepository.UpdateAsync(order);
        }

        private async Task<List<ShipmentExcelRowDto>> ParseShipmentExcelAsync(IFormFile file)
        {
            var rows = new List<ShipmentExcelRowDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            // Expect: Row 1 = Header (Model, SerialNumber), Data starts from Row 2
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var model = worksheet.Cells[row, 1].Text?.Trim();
                var serial = worksheet.Cells[row, 2].Text?.Trim();

                if (!string.IsNullOrWhiteSpace(model) && !string.IsNullOrWhiteSpace(serial))
                {
                    rows.Add(new ShipmentExcelRowDto
                    {
                        Model = model,
                        SerialNumber = serial
                    });
                }
            }

            return await Task.FromResult(rows);
        }

        private async Task<List<ReceiptExcelRowDto>> ParseReceiptExcelAsync(IFormFile file)
        {
            var rows = new List<ReceiptExcelRowDto>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var stream = file.OpenReadStream();
            using var package = new ExcelPackage(stream);
            var worksheet = package.Workbook.Worksheets[0];

            // Expect: Row 1 = Header (Model, SerialNumber), Data starts from Row 2
            for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
            {
                var model = worksheet.Cells[row, 1].Text?.Trim();
                var serial = worksheet.Cells[row, 2].Text?.Trim();

                if (!string.IsNullOrWhiteSpace(model) && !string.IsNullOrWhiteSpace(serial))
                {
                    rows.Add(new ReceiptExcelRowDto
                    {
                        Model = model,
                        SerialNumber = serial
                    });
                }
            }

            return await Task.FromResult(rows);
        }
    }
}
