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
    public class PartOrderIssueService : IPartOrderIssueService
    {
        private readonly IPartOrderRepository _partOrderRepository;
        private readonly IPartOrderItemRepository _partOrderItemRepository;
        private readonly IPartOrderIssueRepository _issueRepository;
        private readonly IPartOrderDiscrepancyResolutionRepository _resolutionRepository;
        private readonly IPartOrderShipmentRepository _shipmentRepository;
        private readonly IPartOrderReceiptRepository _receiptRepository;
        private readonly IVehiclePartHistoryRepository _vehiclePartHistoryRepository;
        private readonly IPartRepository _partRepository;
        private readonly IOrganizationRepository _organizationRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly ICurrentUserService _currentUserService;
        private readonly IPartOrderImageService _imageService;

        public PartOrderIssueService(
            IPartOrderRepository partOrderRepository,
            IPartOrderItemRepository partOrderItemRepository,
            IPartOrderIssueRepository issueRepository,
            IPartOrderDiscrepancyResolutionRepository resolutionRepository,
            IPartOrderShipmentRepository shipmentRepository,
            IPartOrderReceiptRepository receiptRepository,
            IVehiclePartHistoryRepository vehiclePartHistoryRepository,
            IPartRepository partRepository,
            IOrganizationRepository organizationRepository,
            IEmployeeRepository employeeRepository,
            ICurrentUserService currentUserService,
            IPartOrderImageService imageService)
        {
            _partOrderRepository = partOrderRepository;
            _partOrderItemRepository = partOrderItemRepository;
            _issueRepository = issueRepository;
            _resolutionRepository = resolutionRepository;
            _shipmentRepository = shipmentRepository;
            _receiptRepository = receiptRepository;
            _vehiclePartHistoryRepository = vehiclePartHistoryRepository;
            _partRepository = partRepository;
            _organizationRepository = organizationRepository;
            _employeeRepository = employeeRepository;
            _currentUserService = currentUserService;
            _imageService = imageService;
        }

        public async Task<IEnumerable<CancellationReasonDto>> GetCancellationReasonsAsync()
        {
            var reasons = PartOrderCancellationReasonExtensions.GetAllReasons();
            return await Task.FromResult(reasons.Select((r, index) => new CancellationReasonDto
            {
                Value = ((PartOrderCancellationReason)index).ToString(),
                Description = r
            }));
        }

        public async Task<IEnumerable<ReturnReasonDto>> GetReturnReasonsAsync()
        {
            var reasons = PartOrderReturnReasonExtensions.GetAllReasons();
            return await Task.FromResult(reasons.Select((r, index) => new ReturnReasonDto
            {
                Value = ((PartOrderReturnReason)index).ToString(),
                Description = r
            }));
        }

        public async Task CancelShipmentAsync(CancelShipmentRequestDto request)
        {
            // Validate order exists and is in InTransit
            var order = await _partOrderRepository.GetPartOrderByIdAsync(request.OrderId);
            if (order == null)
                throw new ApiException(ResponseError.InvalidOrderId);

            if (order.Status != PartOrderStatus.InTransit.GetPartOrderStatus())
                throw new ApiException(ResponseError.CannotCancelOrder);

            // Validate reason
            if (!Enum.TryParse<PartOrderCancellationReason>(request.Reason, out var reason))
                throw new ApiException(ResponseError.InvalidIssueReason);

            // If reason is Other, ReasonDetail is required
            if (reason == PartOrderCancellationReason.Other && string.IsNullOrWhiteSpace(request.ReasonDetail))
                throw new ApiException(ResponseError.ReasonDetailRequired);

            var userId = _currentUserService.GetUserId();

            // Create issue record
            var issue = new PartOrderIssue
            {
                IssueId = Guid.NewGuid(),
                OrderId = request.OrderId,
                IssueType = "Cancellation",
                Reason = reason.GetDescription(),
                ReasonDetail = request.ReasonDetail,
                Note = request.Note,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _issueRepository.CreateAsync(issue);

            // Update order status to Cancelled
            order.Status = PartOrderStatus.Cancelled.GetPartOrderStatus();
            await _partOrderRepository.UpdateAsync(order);

            // H?Y LÔ HÀNG = M?T H?T
            // Không c?n x? lý gì thêm, admin s? quy?t ??nh b?i th??ng/x? lý sau
            // Stock EVM ?ã b? tr? khi confirm shipment, gi? coi nh? m?t
        }

        public async Task ReturnShipmentAsync(ReturnShipmentRequestDto request)
        {
            // Validate order exists and is in InTransit
            var order = await _partOrderRepository.GetPartOrderByIdAsync(request.OrderId);
            if (order == null)
                throw new ApiException(ResponseError.InvalidOrderId);

            if (order.Status != PartOrderStatus.InTransit.GetPartOrderStatus())
                throw new ApiException(ResponseError.CannotReturnOrder);

            // Validate reason
            if (!Enum.TryParse<PartOrderReturnReason>(request.Reason, out var reason))
                throw new ApiException(ResponseError.InvalidIssueReason);

            // If reason is Other, ReasonDetail is required
            if (reason == PartOrderReturnReason.Other && string.IsNullOrWhiteSpace(request.ReasonDetail))
                throw new ApiException(ResponseError.ReasonDetailRequired);

            var userId = _currentUserService.GetUserId();

            // Create issue record
            var issue = new PartOrderIssue
            {
                IssueId = Guid.NewGuid(),
                OrderId = request.OrderId,
                IssueType = "Return",
                Reason = reason.GetDescription(),
                ReasonDetail = request.ReasonDetail,
                Note = request.Note,
                CreatedBy = userId,
                CreatedAt = DateTime.UtcNow
            };

            await _issueRepository.CreateAsync(issue);

            // Update order status to Returning
            order.Status = PartOrderStatus.Returning.GetPartOrderStatus();
            await _partOrderRepository.UpdateAsync(order);

            // TR? HÀNG = HÀNG QUAY V? KHO EVM
            // S? c?n validate và confirm return receipt nh? bình th??ng
        }

        public async Task<ReceiptValidationResultDto> ValidateReturnReceiptAsync(Guid orderId, IFormFile file)
        {
            var result = new ReceiptValidationResultDto { IsValid = true };

            // Validate order exists and is in Returning status
            var order = await _partOrderRepository.GetPartOrderByIdAsync(orderId);
            if (order == null)
                throw new ApiException(ResponseError.InvalidOrderId);

            if (order.Status != PartOrderStatus.Returning.GetPartOrderStatus())
                throw new ApiException(ResponseError.CannotReturnOrder);

            // Get shipments (what was sent)
            var shipments = await _shipmentRepository.GetByOrderIdAsync(orderId);
            if (!shipments.Any())
                throw new ApiException(ResponseError.ShipmentNotValidated);

            var shippedSerials = shipments.ToDictionary(s => s.SerialNumber, s => s);

            // Parse Excel file
            List<ReceiptExcelRowDto> excelRows;
            try
            {
                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using var stream = file.OpenReadStream();
                using var package = new ExcelPackage(stream);
                var worksheet = package.Workbook.Worksheets[0];

                excelRows = new List<ReceiptExcelRowDto>();
                for (int row = 2; row <= worksheet.Dimension.End.Row; row++)
                {
                    var model = worksheet.Cells[row, 1].Text?.Trim();
                    var serial = worksheet.Cells[row, 2].Text?.Trim();

                    if (!string.IsNullOrWhiteSpace(model) && !string.IsNullOrWhiteSpace(serial))
                    {
                        excelRows.Add(new ReceiptExcelRowDto
                        {
                            Model = model,
                            SerialNumber = serial
                        });
                    }
                }
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
            var returnedQuantities = excelRows.GroupBy(r => r.Model)
                .ToDictionary(g => g.Key, g => g.Count());

            var shippedQuantities = shipments.GroupBy(s => s.Model)
                .ToDictionary(g => g.Key, g => g.Count());

            // Check quantity discrepancies
            foreach (var kvp in shippedQuantities)
            {
                var model = kvp.Key;
                var shipped = kvp.Value;
                var returned = returnedQuantities.GetValueOrDefault(model, 0);
                var difference = returned - shipped;

                var discrepancy = new QuantityDiscrepancyDto
                {
                    Model = model,
                    Requested = shipped,
                    Provided = returned,
                    Difference = difference,
                    Status = difference == 0 ? "Match" : (difference > 0 ? "Excess" : "Shortage")
                };

                if (difference != 0)
                {
                    result.IsValid = false;
                    result.QuantityDiscrepancies[model] = discrepancy;
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

            // Delete old return receipts if any
            await _receiptRepository.DeleteByOrderIdAsync(orderId);

            // If valid, save return receipts
            if (result.IsValid)
            {
                var receipts = excelRows.Select(r => new PartOrderReceipt
                {
                    ReceiptId = Guid.NewGuid(),
                    OrderId = orderId,
                    Model = r.Model,
                    SerialNumber = r.SerialNumber,
                    ReceivedAt = DateTime.UtcNow,
                    Status = "Received"
                }).ToList();

                await _receiptRepository.AddRangeAsync(receipts);

                // Update order status to ReturnInspection
                order.Status = PartOrderStatus.ReturnInspection.GetPartOrderStatus();
                await _partOrderRepository.UpdateAsync(order);
            }

            return result;
        }

        public async Task ConfirmReturnReceiptAsync(Guid orderId, string? damagedPartsJson, List<IFormFile>? images)
        {
            // Validate order exists and is in ReturnInspection
            var order = await _partOrderRepository.GetPartOrderByIdAsync(orderId);
            if (order == null)
                throw new ApiException(ResponseError.InvalidOrderId);

            if (order.Status != PartOrderStatus.ReturnInspection.GetPartOrderStatus())
                throw new ApiException(ResponseError.OrderNotInReturnInspection);

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

            // Get EVM org ID
            var evmOrgId = await _currentUserService.GetOrgId();

            // Get good receipts
            var goodReceipts = receipts.Where(r => r.Status == "Received").ToList();

            // Update VehiclePartHistory for returned good items
            foreach (var receipt in goodReceipts)
            {
                var partHistory = await _vehiclePartHistoryRepository.GetBySerialNumberAsync(receipt.SerialNumber);
                if (partHistory != null)
                {
                    // Already has correct ServiceCenterId (EVM), just ensure status
                    partHistory.Status = VehiclePartCurrentStatus.InStock.GetCurrentStatus();
                }
            }

            // Update EVM Part stock (add back good parts only)
            var groupedByModel = goodReceipts.GroupBy(r => r.Model)
                .ToDictionary(g => g.Key, g => g.Count());

            var partsToUpdate = new List<Part>();
            foreach (var kvp in groupedByModel)
            {
                var part = await _partRepository.GetPartsAsync(kvp.Key, evmOrgId);
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

            // Check if there are discrepancies (damaged or missing parts)
            var hasDiscrepancies = receipts.Any(r => r.Status == "Damaged") || 
                                   receipts.Count < (await _shipmentRepository.GetByOrderIdAsync(orderId)).Count();

            if (hasDiscrepancies)
            {
                // Create discrepancy resolution record
                var resolution = new PartOrderDiscrepancyResolution
                {
                    ResolutionId = Guid.NewGuid(),
                    OrderId = orderId,
                    Status = DiscrepancyResolutionStatus.PendingResolution.GetStatus(),
                    CreatedAt = DateTime.UtcNow
                };

                await _resolutionRepository.CreateAsync(resolution);

                // Update order status to DiscrepancyReview
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

        public async Task ResolveDiscrepancyAsync(ResolveDiscrepancyRequestDto request)
        {
            // Validate order exists
            var order = await _partOrderRepository.GetPartOrderByIdAsync(request.OrderId);
            if (order == null)
                throw new ApiException(ResponseError.InvalidOrderId);

            // Get resolution record
            var resolution = await _resolutionRepository.GetByOrderIdAsync(request.OrderId);
            if (resolution == null)
                throw new ApiException(ResponseError.NoDiscrepancyToResolve);

            // Validate responsible party
            var validParties = new[] { "EVM", "SC", "Transport", "Shared" };
            if (!validParties.Contains(request.ResponsibleParty))
                throw new ApiException(ResponseError.InvalidResponsibleParty);

            var userId = _currentUserService.GetUserId();

            // Update resolution
            resolution.Status = DiscrepancyResolutionStatus.Resolved.GetStatus();
            resolution.ResponsibleParty = request.ResponsibleParty;
            resolution.Decision = request.Decision;
            resolution.Note = request.Note;
            resolution.ResolvedBy = userId;
            resolution.ResolvedAt = DateTime.UtcNow;

            await _resolutionRepository.UpdateAsync(resolution);

            // Update order status to Done
            order.Status = PartOrderStatus.Done.GetPartOrderStatus();
            await _partOrderRepository.UpdateAsync(order);
        }

        public async Task<IEnumerable<DiscrepancyResolutionDto>> GetPendingDiscrepanciesAsync()
        {
            var resolutions = await _resolutionRepository.GetPendingResolutionsAsync();
            var result = new List<DiscrepancyResolutionDto>();

            foreach (var resolution in resolutions)
            {
                string? resolvedByName = null;
                if (resolution.ResolvedBy.HasValue)
                {
                    var employee = await _employeeRepository.GetEmployeeByIdAsync(resolution.ResolvedBy.Value);
                    resolvedByName = employee?.Name;
                }

                result.Add(new DiscrepancyResolutionDto
                {
                    ResolutionId = resolution.ResolutionId,
                    OrderId = resolution.OrderId,
                    Status = resolution.Status,
                    ResponsibleParty = resolution.ResponsibleParty,
                    Decision = resolution.Decision,
                    Note = resolution.Note,
                    ResolvedBy = resolution.ResolvedBy,
                    ResolvedByName = resolvedByName,
                    ResolvedAt = resolution.ResolvedAt,
                    CreatedAt = resolution.CreatedAt
                });
            }

            return result;
        }

        public async Task<Guid> CreatePartOrderByEvmAsync(CreatePartOrderByEvmRequestDto request)
        {
            // Validate service center exists
            var serviceCenter = await _organizationRepository.GetOrganizationById(request.ServiceCenterId);
            if (serviceCenter == null)
                throw new ApiException(ResponseError.ServiceCenterNotFound);

            // Validate items
            if (request.Items == null || !request.Items.Any())
                throw new ApiException(ResponseError.InvalidJsonFormat);

            // Validate quantities
            foreach (var item in request.Items)
            {
                if (item.Quantity > PartOrderConstants.MAX_QUANTITY_PER_PART)
                    throw new ApiException(ResponseError.PartOrderQuantityExceedsMax);
            }

            var userId = _currentUserService.GetUserId();

            // Create new PartOrder
            var order = new PartOrder
            {
                OrderId = Guid.NewGuid(),
                ServiceCenterId = request.ServiceCenterId,
                RequestDate = DateTime.UtcNow,
                Status = PartOrderStatus.Pending.GetPartOrderStatus(),
                CreatedBy = userId
            };

            await _partOrderRepository.CreateAsync(order);

            // Create PartOrderItems
            foreach (var item in request.Items)
            {
                var orderItem = new PartOrderItem
                {
                    OrderItemId = Guid.NewGuid(),
                    OrderId = order.OrderId,
                    Model = item.Model,
                    Quantity = item.Quantity,
                    Remarks = item.Remarks ?? string.Empty
                };

                await _partOrderItemRepository.CreateAsync(orderItem);
            }

            return order.OrderId;
        }
    }
}
