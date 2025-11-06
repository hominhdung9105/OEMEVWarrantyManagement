using Bogus;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        //TODO - check logic
        public static void SeedDatabase(AppDbContext context)
        {
            // --- CÀI ĐẶT ---
            const int primaryRecordCount = 100;
            const int joinRecordCount = 300;
            const int warrantyCLaimCount = 500;
            //const int employeeCount = 20;
            //const int organizationCount = 5;
            Randomizer.Seed = new Random(8675309);

            if (context.Organizations.Any())
            {
                return; // DB đã có dữ liệu
            }

            // --- CẤP 1: BẢNG KHÔNG PHỤ THUỘC ---

            // --- SỬA LOGIC TẠO ORGANIZATION ---
            // 1. Tạo Faker chung (không có Type)
            var baseOrgFaker = new Faker<Organization>("en")
                .RuleFor(o => o.OrgId, f => f.Database.Random.Guid())
                .RuleFor(o => o.Name, f => f.Company.CompanyName())
                // .RuleFor(o => o.Type, ...) // Xóa dòng chọn Type ngẫu nhiên
                .RuleFor(o => o.Region, f => f.Address.State())
                .RuleFor(o => o.ContactInfo, f => f.Phone.PhoneNumber());

            // 2. Tạo 1 OEM (EVM)
            var oems = baseOrgFaker
                .RuleFor(o => o.Type, "OEM") // Gán cứng Type là OEM
                .Generate(1);               // Chỉ 1 OEM

            // 3. Tạo 3 ServiceCenter
            var serviceCenters = baseOrgFaker
                .RuleFor(o => o.Type, "ServiceCenter") // Gán cứng Type là ServiceCenter
                .Generate(3);                      // Tạo 3 bản ghi

            // 4. Gộp danh sách và lưu
            var organizations = new List<Organization>();
            organizations.AddRange(oems);
            organizations.AddRange(serviceCenters);

            context.Organizations.AddRange(organizations);
            // ------------------------------------

            context.SaveChanges(); // Lưu cả organizations

            // --- CẤP 2: PHỤ THUỘC VÀO CẤP 1 ---

            // --- SỬA LOGIC TẠO EMPLOYEE ---
            // 1. Lấy danh sách OEM và ServiceCenter đã tạo
            var oemOrgs = organizations.Where(o => o.Type == "OEM").ToList();
            var scOrgs = organizations.Where(o => o.Type == "ServiceCenter").ToList();
            var singleOem = oemOrgs.First();

            // 2. Tạo Faker cơ bản cho Employee
            var baseEmployeeFaker = new Faker<Employee>("en")
                .RuleFor(e => e.UserId, f => f.Database.Random.Guid())
                .RuleFor(e => e.Email, (f, u) => f.Internet.Email(f.Name.FirstName(), f.Name.LastName()))
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.PasswordHash, f => "pass123");

            // 3. Tạo danh sách tổng hợp employee
            var employees = new List<Employee>();

            // 4. Tạo 1 ADMIN (gán vào OEM duy nhất)
            var admin = baseEmployeeFaker
                .RuleFor(e => e.Role, "ADMIN")
                .RuleFor(e => e.OrgId, singleOem.OrgId) // Gán vào OEM duy nhất
                .Generate();
            employees.Add(admin);

            // 5. Tạo đúng 1 EVM_STAFF (gán vào OEM duy nhất)
            var evmStaff = baseEmployeeFaker
                .RuleFor(e => e.Role, "EVM_STAFF")
                .RuleFor(e => e.OrgId, singleOem.OrgId)
                .Generate(1);
            employees.AddRange(evmStaff);

            // 6. Tạo SC_STAFF và SC_TECH cho từng ServiceCenter
            foreach (var sc in scOrgs)
            {
                // 2 SC_STAFF
                var scStaff = baseEmployeeFaker
                        .RuleFor(e => e.Role, "SC_STAFF")
                        .RuleFor(e => e.OrgId, sc.OrgId)
                        .Generate(2);
                employees.AddRange(scStaff);

                // 5 SC_TECH
                var scTech = baseEmployeeFaker
                        .RuleFor(e => e.Role, "SC_TECH")
                        .RuleFor(e => e.OrgId, sc.OrgId)
                        .Generate(5);
                employees.AddRange(scTech);
            }

            // 7. Lưu tất cả Employee đã tạo
            context.Employees.AddRange(employees);

            var customerFaker = new Faker<Customer>("en")
                .RuleFor(c => c.CustomerId, f => f.Database.Random.Guid())
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.OrganizationOrgId, (f, u) => f.PickRandom(organizations).OrgId);
            var customers = customerFaker.Generate(primaryRecordCount);
            context.Customers.AddRange(customers);

            // --- SỬA LOGIC TẠO PART ---
            // Kho của EVM có tất cả model với số lượng lớn.
            // TẤT CẢ Service Center CŨNG có đủ các model đó nhưng số lượng ít hoặc bằng 0.
            var allModels = PartModel.ModelsByCategory.SelectMany(kvp => kvp.Value).Distinct().ToList();
            var rng = new Random();
            var parts = new List<Part>();

            // 1) Seed tất cả model cho OEM với stock lớn
            foreach (var model in allModels)
            {
                var category = PartModel.GetCategoryByModel(model) ?? PartCategory.Other.GetPartCategory();
                parts.Add(new Part
                {
                    PartId = Guid.NewGuid(),
                    Model = model,
                    Name = $"{category} - {model}",
                    Category = category,
                    StockQuantity = rng.Next(800, 2001), // số lượng lớn
                    OrgId = singleOem.OrgId
                });
            }

            // 2) Mỗi Service Center có đủ các model với stock thấp hoặc 0
            foreach (var sc in scOrgs)
            {
                foreach (var model in allModels)
                {
                    var category = PartModel.GetCategoryByModel(model) ?? PartCategory.Other.GetPartCategory();
                    parts.Add(new Part
                    {
                        PartId = Guid.NewGuid(),
                        Model = model,
                        Name = $"{category} - {model}",
                        Category = category,
                        StockQuantity = rng.Next(0, 51), // số lượng ít hoặc bằng 0
                        OrgId = sc.OrgId
                    });
                }
            }

            context.Parts.AddRange(parts);

            var policyFaker = new Faker<WarrantyPolicy>("en")
                .RuleFor(p => p.PolicyId, f => f.Database.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName() + " Warranty")
                .RuleFor(p => p.CoveragePeriodMonths, f => f.PickRandom(new[] { 12, 24, 36, 48 }))
                .RuleFor(p => p.Conditions, f => f.Lorem.Paragraph())
                .RuleFor(p => p.OrganizationOrgId, (f, u) => f.PickRandom(organizations).OrgId);
            var policies = policyFaker.Generate(primaryRecordCount);
            context.WarrantyPolicies.AddRange(policies);

            var campaignFaker = new Faker<Campaign>("en")
                .RuleFor(c => c.CampaignId, f => f.Database.Random.Guid())
                .RuleFor(c => c.Title, f => f.Company.Bs() + " Campaign")
                .RuleFor(c => c.Type, f => f.PickRandom(new[] { "RECALL", "SERVICE" }))
                .RuleFor(c => c.Description, f => f.Lorem.Sentence())
                .RuleFor(c => c.StartDate, f => f.Date.Past(1))
                .RuleFor(c => c.EndDate, f => f.Date.Future(1))
                .RuleFor(c => c.Status, f => f.PickRandom(new[] { "Active", "Close" }))
                .RuleFor(c => c.OrganizationOrgId, (f, u) => f.PickRandom(organizations).OrgId);
            var campaigns = campaignFaker.Generate(primaryRecordCount);
            context.Campaigns.AddRange(campaigns);

            // --- WORKORDER ĐÃ BỊ XÓA KHỎI ĐÂY ---

            var scEmployeeIds = employees.Where(e => e.Role == "SC_STAFF" || e.Role == "SC_TECH").Select(e => e.UserId).ToList();
            var partOrderFaker = new Faker<PartOrder>("en")
                .RuleFor(p => p.OrderId, f => f.Database.Random.Guid())
                // Chỉ chọn ServiceCenter làm ServiceCenterId
                .RuleFor(p => p.ServiceCenterId, (f, u) => f.PickRandom(scOrgs).OrgId)
                .RuleFor(p => p.RequestDate, f => f.Date.Past(1))
                .RuleFor(p => p.ApprovedDate, (f, p) => f.Random.Bool(0.7f) ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(p => p.ShippedDate, (f, p) => p.ApprovedDate.HasValue ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(p => p.ExpectedDate, (f, p) => DateOnly.FromDateTime(p.RequestDate.AddDays(f.Random.Int(5, 15))))
                .RuleFor(p => p.PartDelivery, (f, p) => p.ShippedDate.HasValue ? p.ExpectedDate?.ToDateTime(new TimeOnly(0,0)).AddDays(f.Random.Int(-1, 3)) : (DateTime?)null)
                .RuleFor(p => p.Status, f => f.PickRandom(new[] { "Pending", "Waiting", "Deliverd", "Done" }))
                // Người tạo ưu tiên nhân viên SC
                .RuleFor(p => p.CreatedBy, (f, u) => scEmployeeIds.Count > 0 ? f.PickRandom(scEmployeeIds) : f.PickRandom(employees).UserId);
            var partOrders = partOrderFaker.Generate(primaryRecordCount);
            context.PartOrders.AddRange(partOrders);

            context.SaveChanges(); // Lưu thay đổi CẤP 2

            // --- CẤP 3: PHỤ THUỘC VÀO CẤP 2 ---
            var vehicleFaker = new Faker<Vehicle>("en")
                .RuleFor(v => v.Vin, f => f.Random.AlphaNumeric(17).ToUpper())
                .RuleFor(v => v.Model, f => f.Vehicle.Model())
                .RuleFor(v => v.Year, f => f.Random.Int(2020, 2025))
                .RuleFor(v => v.CustomerId, (f, u) => f.PickRandom(customers).CustomerId);
            var vehicles = vehicleFaker.Generate(primaryRecordCount);
            context.Vehicles.AddRange(vehicles);

            var campaignTargetFaker = new Faker<CampaignTarget>("en")
                .RuleFor(ct => ct.CampaignTargetId, f => f.Database.Random.Guid())
                .RuleFor(ct => ct.CampaignId, (f, u) => f.PickRandom(campaigns).CampaignId)
                .RuleFor(ct => ct.TargetType, f => f.PickRandom(new[] { "Model", "YearRange", "VIN" }))
                .RuleFor(ct => ct.TargetRefId, f => f.Database.Random.Guid())
                .RuleFor(ct => ct.YearFrom, (f, ct) => ct.TargetType == "YearRange" ? f.Random.Int(2018, 2020) : (int?)null)
                .RuleFor(ct => ct.YearTo, (f, ct) => ct.TargetType == "YearRange" ? f.Random.Int(2021, 2023) : (int?)null);
            var campaignTargets = campaignTargetFaker.Generate(joinRecordCount);
            context.CampaignTargets.AddRange(campaignTargets);

            var partOrderItemFaker = new Faker<PartOrderItem>("en")
                .RuleFor(poi => poi.OrderItemId, f => f.Database.Random.Guid())
                .RuleFor(poi => poi.OrderId, (f, u) => f.PickRandom(partOrders).OrderId)
                .RuleFor(poi => poi.Model, (f, u) => f.PickRandom(parts).Model)
                .RuleFor(poi => poi.Quantity, f => f.Random.Int(1, 10))
                .RuleFor(poi => poi.Remarks, f => f.Lorem.Sentence());
            var partOrderItems = partOrderItemFaker.Generate(joinRecordCount);
            context.PartOrderItems.AddRange(partOrderItems);

            context.SaveChanges(); // Lưu thay đổi CẤP 3

            // --- CẤP 4: PHỤ THUỘC VÀO CẤP 3 ---
            var campaignVehicleFaker = new Faker<CampaignVehicle>("en")
                .RuleFor(cv => cv.CampaignVehicleId, f => f.Database.Random.Guid())
                .RuleFor(cv => cv.CampaignId, (f, u) => f.PickRandom(campaigns).CampaignId)
                .RuleFor(cv => cv.Vin, (f, u) => f.PickRandom(vehicles).Vin)
                .RuleFor(cv => cv.NotifyToken, f => f.Random.AlphaNumeric(20))
                .RuleFor(cv => cv.Status, f => f.PickRandom(new[] { "PENDING", "NOTIFIED", "CONFIRMED", "DONE", "CANCELLED" }))
                .RuleFor(cv => cv.NotifiedAt, (f, cv) => cv.Status is "NOTIFIED" or "CONFIRMED" or "DONE" ? f.Date.Past(1) : (DateTime?)null)
                .RuleFor(cv => cv.ConfirmedAt, (f, cv) => cv.Status is "CONFIRMED" or "DONE" ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(cv => cv.CompletedAt, (f, cv) => cv.Status == "DONE" ? f.Date.Recent() : (DateTime?)null);
            var campaignVehicles = campaignVehicleFaker.Generate(joinRecordCount);
            context.CampaignVehicles.AddRange(campaignVehicles);

            // VehiclePart: đảm bảo một số xe có rất nhiều part
            var vehicleParts = new List<VehiclePart>();
            var rng2 = new Random();
            var manyPartsVehicles = vehicles.OrderBy(_ => rng2.Next()).Take(5).ToList(); // 5 xe có nhiều part

            foreach (var v in manyPartsVehicles)
            {
                var count = rng2.Next(25, 41); // 25-40 part mỗi xe
                for (int i = 0; i < count; i++)
                {
                    var selectedPart = parts[rng2.Next(parts.Count)];
                    var installed = DateTime.UtcNow.AddDays(-rng2.Next(30, 800));
                    var isUninstalled = rng2.NextDouble() < 0.5; // 50% đã tháo
                    var uninstalled = isUninstalled ? installed.AddDays(rng2.Next(1, 5 * 365)) : DateTime.MinValue;

                    vehicleParts.Add(new VehiclePart
                    {
                        VehiclePartId = Guid.NewGuid(),
                        Vin = v.Vin,
                        Model = selectedPart.Model,
                        SerialNumber = new string(Enumerable.Range(0, 12).Select(_ => (char)rng2.Next('A', 'Z' + 1)).ToArray()),
                        InstalledDate = installed,
                        UninstalledDate = uninstalled,
                        Status = isUninstalled ? VehiclePartStatus.UnInstalled.GetVehiclePartStatus() : VehiclePartStatus.Installed.GetVehiclePartStatus()
                    });
                }
            }

            // Các xe còn lại: 0-5 part
            var remainingVehicles = vehicles.Except(manyPartsVehicles).ToList();
            foreach (var v in remainingVehicles)
            {
                var count = rng2.Next(0, 6);
                for (int i = 0; i < count; i++)
                {
                    var selectedPart = parts[rng2.Next(parts.Count)];
                    var installed = DateTime.UtcNow.AddDays(-rng2.Next(30, 800));
                    var isUninstalled = rng2.NextDouble() < 0.3; // 30% đã tháo
                    var uninstalled = isUninstalled ? installed.AddDays(rng2.Next(1, 5 * 365)) : DateTime.MinValue;

                    vehicleParts.Add(new VehiclePart
                    {
                        VehiclePartId = Guid.NewGuid(),
                        Vin = v.Vin,
                        Model = selectedPart.Model,
                        SerialNumber = new string(Enumerable.Range(0, 12).Select(_ => (char)rng2.Next('A', 'Z' + 1)).ToArray()),
                        InstalledDate = installed,
                        UninstalledDate = uninstalled,
                        Status = isUninstalled ? VehiclePartStatus.UnInstalled.GetVehiclePartStatus() : VehiclePartStatus.Installed.GetVehiclePartStatus()
                    });
                }
            }

            context.VehicleParts.AddRange(vehicleParts);

            var vehiclePolicyFaker = new Faker<VehicleWarrantyPolicy>("en")
                .RuleFor(vp => vp.VehicleWarrantyId, f => f.Database.Random.Guid())
                .RuleFor(vp => vp.Vin, (f, u) => f.PickRandom(vehicles).Vin)
                .RuleFor(vp => vp.PolicyId, (f, u) => f.PickRandom(policies).PolicyId)
                .RuleFor(vp => vp.StartDate, f => f.Date.Past(2))
                .RuleFor(vp => vp.EndDate, (f, vp) => vp.StartDate.AddMonths(f.PickRandom(policies).CoveragePeriodMonths))
                .RuleFor(vp => vp.Status, f => "Active");
            var vehiclePolicies = vehiclePolicyFaker.Generate(joinRecordCount);
            context.VehicleWarrantyPolicies.AddRange(vehiclePolicies);

            var claimFaker = new Faker<WarrantyClaim>("en")
                .RuleFor(c => c.ClaimId, f => f.Database.Random.Guid())
                .RuleFor(c => c.Vin, (f, u) => f.PickRandom(vehicles).Vin)
                .RuleFor(c => c.ServiceCenterId, (f, u) => f.PickRandom(scOrgs).OrgId)
                .RuleFor(c => c.CreatedBy, (f, u) => f.PickRandom(employees).UserId)
                .RuleFor(c => c.CreatedDate, f => f.Date.Past(1))
                .RuleFor(c => c.Status, f => f.PickRandom(new[] { "waiting for unassigned", "under inspection", "pending confirmation", "sent to manufacturer", "denied", "approved", "waiting for unassigned repair", "under repair", "repaired", "car back home", "hold customer car", "done warranty" }))
                .RuleFor(c => c.Description, f => f.Lorem.Paragraph())
                .RuleFor(c => c.ConfirmBy, (f, c) => c.Status == "approved" || c.Status == "denied" ? f.PickRandom(employees).UserId : (Guid?)null)
                .RuleFor(c => c.ConfirmDate, (f, c) => c.ConfirmBy.HasValue ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(c => c.VehicleWarrantyId, (f, u) => f.PickRandom(vehiclePolicies).VehicleWarrantyId)
                .RuleFor(c => c.failureDesc, f => f.Lorem.Sentence());
            var claims = claimFaker.Generate(warrantyCLaimCount);
            context.WarrantyClaims.AddRange(claims);

            // === DEMO FLOW (Luồng 1) ===
            // Chọn 1 Service Center, 1 SC_STAFF và 1 SC_TECH cố định
            var demoSc = scOrgs.First();
            var demoScStaff = employees.First(e => e.Role == "SC_STAFF" && e.OrgId == demoSc.OrgId);
            var demoScTech = employees.First(e => e.Role == "SC_TECH" && e.OrgId == demoSc.OrgId);
            var demoEvmStaff = employees.First(e => e.Role == "EVM_STAFF");

            // Chọn 1 vehicle làm demo, đảm bảo có ít nhất 2 part đang Installed
            var demoVehicle = vehicles.First();
            var demoVehicleParts = vehicleParts.Where(vp => vp.Vin == demoVehicle.Vin && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus()).ToList();
            if (demoVehicleParts.Count < 2)
            {
                // Bổ sung thêm parts nếu thiếu
                var pickModels = parts.Select(p => p.Model).Distinct().Take(2).ToList();
                foreach (var m in pickModels)
                {
                    var vp = new VehiclePart
                    {
                        VehiclePartId = Guid.NewGuid(),
                        Vin = demoVehicle.Vin,
                        Model = m,
                        SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                        InstalledDate = DateTime.UtcNow.AddDays(-30),
                        UninstalledDate = DateTime.MinValue,
                        Status = VehiclePartStatus.Installed.GetVehiclePartStatus()
                    };
                    vehicleParts.Add(vp);
                    context.VehicleParts.Add(vp);
                }
                context.SaveChanges();
                demoVehicleParts = vehicleParts.Where(vp => vp.Vin == demoVehicle.Vin && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus()).Take(2).ToList();
            }

            // Tạo 1 policy đang active cho vehicle demo (nếu chưa có)
            var demoPolicy = new VehicleWarrantyPolicy
            {
                VehicleWarrantyId = Guid.NewGuid(),
                Vin = demoVehicle.Vin,
                PolicyId = policies.First().PolicyId,
                StartDate = DateTime.UtcNow.AddMonths(-6),
                EndDate = DateTime.UtcNow.AddMonths(18),
                Status = "Active"
            };
            context.VehicleWarrantyPolicies.Add(demoPolicy);

            // Claim 1: đang under inspection + WorkOrder Inspection in progress
            var claimInspection = new WarrantyClaim
            {
                ClaimId = Guid.NewGuid(),
                Vin = demoVehicle.Vin,
                ServiceCenterId = demoSc.OrgId,
                CreatedBy = demoScStaff.UserId,
                CreatedDate = DateTime.UtcNow.AddDays(-2),
                Status = WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus(),
                Description = "Inspection for abnormal noise",
                failureDesc = "Noise from motor area"
            };
            context.WarrantyClaims.Add(claimInspection);

            // ClaimPart cho Claim 1: Replace 1 part với SerialNumberOld khớp VehiclePart
            var demoPart1 = demoVehicleParts.First();
            var claim1Part = new ClaimPart
            {
                ClaimPartId = Guid.NewGuid(),
                ClaimId = claimInspection.ClaimId,
                Model = demoPart1.Model,
                SerialNumberOld = demoPart1.SerialNumber,
                SerialNumberNew = null,
                Action = ClaimPartAction.Replace.GetClaimPartAction(),
                Status = ClaimPartStatus.Pending.GetClaimPartStatus(),
                Cost = 0
            };
            context.ClaimParts.Add(claim1Part);

            var woInspection = new WorkOrder
            {
                WorkOrderId = Guid.NewGuid(),
                AssignedTo = demoScTech.UserId,
                Type = WorkOrderType.Inspection.GetWorkOrderType(),
                Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                TargetId = claimInspection.ClaimId,
                Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                StartDate = DateTime.UtcNow.AddDays(-1),
                Notes = "Initial inspection assigned"
            };
            context.WorkOrders.Add(woInspection);

            // Claim 2: đã approved và đang under repair + WorkOrder Repair in progress
            var claimRepair = new WarrantyClaim
            {
                ClaimId = Guid.NewGuid(),
                Vin = demoVehicle.Vin,
                ServiceCenterId = demoSc.OrgId,
                CreatedBy = demoScStaff.UserId,
                CreatedDate = DateTime.UtcNow.AddDays(-7),
                Status = WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus(),
                Description = "Approved repair for BMS",
                ConfirmBy = demoEvmStaff.UserId,
                ConfirmDate = DateTime.UtcNow.AddDays(-6),
                VehicleWarrantyId = demoPolicy.VehicleWarrantyId,
                failureDesc = "BMS frequently resets"
            };
            context.WarrantyClaims.Add(claimRepair);

            var demoPart2 = demoVehicleParts.Skip(1).First();
            var claim2Part = new ClaimPart
            {
                ClaimPartId = Guid.NewGuid(),
                ClaimId = claimRepair.ClaimId,
                Model = demoPart2.Model,
                SerialNumberOld = demoPart2.SerialNumber,
                SerialNumberNew = null,
                Action = ClaimPartAction.Replace.GetClaimPartAction(),
                Status = ClaimPartStatus.Enough.GetClaimPartStatus(),
                Cost = 0
            };
            context.ClaimParts.Add(claim2Part);

            var woRepair = new WorkOrder
            {
                WorkOrderId = Guid.NewGuid(),
                AssignedTo = demoScTech.UserId,
                Type = WorkOrderType.Repair.GetWorkOrderType(),
                Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                TargetId = claimRepair.ClaimId,
                Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                StartDate = DateTime.UtcNow.AddDays(-5),
                Notes = "Repair BMS module"
            };
            context.WorkOrders.Add(woRepair);

            // === Appointments ===
            var appointmentFaker = new Faker<Appointment>("en")
                .RuleFor(a => a.AppointmentId, f => f.Database.Random.Guid())
                .RuleFor(a => a.AppointmentType, f => f.PickRandom(new[] { "Warranty", "Campaign" }))
                .RuleFor(a => a.Vin, f => f.PickRandom(vehicles).Vin)
                .RuleFor(a => a.ServiceCenterId, f => f.PickRandom(scOrgs).OrgId)
                .RuleFor(a => a.AppointmentDate, f => DateOnly.FromDateTime(f.Date.Soon(30)))
                .RuleFor(a => a.Slot, f => f.PickRandom(new[] { "Slot1", "Slot2", "Slot3", "Slot4", "Slot5", "Slot6", "Slot7", "Slot8" }))
                .RuleFor(a => a.Status, f => f.PickRandom(new[] { "Scheduled", "Checked_in", "Canceled", "Done", "No_show", "Pending" }))
                .RuleFor(a => a.CreatedAt, f => f.Date.Recent(60))
                .RuleFor(a => a.Note, f => f.Random.Bool(0.4f) ? f.Lorem.Sentence() : null);

            var appointments = appointmentFaker.Generate(primaryRecordCount);
            context.Appointments.AddRange(appointments);

            context.SaveChanges(); // Lưu thay đổi CẤP 4 + DEMO FLOW + Appointments

            // --- CẤP 5: PHỤ THUỘC VÀO CẤP 4 ---

            // --- WORKORDER RANDOM (đúng Target/TargetId) ---
            var workOrderFaker = new Faker<WorkOrder>("en")
                .RuleFor(w => w.WorkOrderId, f => f.Database.Random.Guid())
                .RuleFor(w => w.AssignedTo, (f, u) => f.PickRandom(employees).UserId)
                .RuleFor(w => w.Type, f => f.PickRandom(new[] { WorkOrderType.Repair.GetWorkOrderType(), WorkOrderType.Inspection.GetWorkOrderType() }))
                .RuleFor(w => w.Status, f => f.PickRandom(new[] { WorkOrderStatus.InProgress.GetWorkOrderStatus(), WorkOrderStatus.Completed.GetWorkOrderStatus() }))
                .RuleFor(w => w.StartDate, f => f.Date.Past(1))
                .RuleFor(w => w.EndDate, (f, w) => w.Status == WorkOrderStatus.Completed.GetWorkOrderStatus() ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(w => w.Notes, f => f.Lorem.Sentence())
                .RuleFor(w => w.Target, f => f.PickRandom(new[] { WorkOrderTarget.Campaign.GetWorkOrderTarget(), WorkOrderTarget.Warranty.GetWorkOrderTarget() }))
                .RuleFor(w => w.TargetId, (f, w) =>
                {
                    if (w.Target == WorkOrderTarget.Warranty.GetWorkOrderTarget())
                    {
                        return f.PickRandom(claims).ClaimId; // đúng: Warranty -> ClaimId
                    }
                    return f.PickRandom(campaigns).CampaignId; // Campaign -> CampaignId
                });
            var workOrders = workOrderFaker.Generate(primaryRecordCount);
            context.WorkOrders.AddRange(workOrders);


            var backClaimFaker = new Faker<BackWarrantyClaim>("en")
                .RuleFor(b => b.WarrantyClaimId, (f, u) => f.PickRandom(claims).ClaimId)
                .RuleFor(b => b.CreatedDate, f => f.Date.Recent())
                .RuleFor(b => b.Description, f => f.Lorem.Sentence(10))
                .RuleFor(b => b.CreatedByEmployeeId, (f, u) => f.PickRandom(employees).UserId);
            var backClaims = backClaimFaker.Generate(joinRecordCount)
                                           .GroupBy(c => new { c.WarrantyClaimId, c.CreatedDate })
                                           .Select(g => g.First())
                                           .ToList();
            context.BackWarrantyClaims.AddRange(backClaims);

            var attachmentFaker = new Faker<ClaimAttachment>("en")
                .RuleFor(a => a.AttachmentId, f => f.Database.Random.Guid().ToString())
                .RuleFor(a => a.ClaimId, (f, u) => f.PickRandom(claims).ClaimId)
                .RuleFor(a => a.URL, f => f.Image.PicsumUrl())
                .RuleFor(a => a.UploadedBy, (f, u) => f.PickRandom(employees).UserId);
            var attachments = attachmentFaker.Generate(joinRecordCount);
            context.ClaimAttachments.AddRange(attachments);

            var claimPartFaker = new Faker<ClaimPart>("en")
                .RuleFor(cp => cp.ClaimPartId, f => f.Database.Random.Guid())
                .RuleFor(cp => cp.ClaimId, (f, u) => f.PickRandom(claims).ClaimId)
                .RuleFor(cp => cp.Model, f => f.PickRandom(parts).Model)
                .RuleFor(cp => cp.SerialNumberOld, f => f.Random.AlphaNumeric(12))
                .RuleFor(cp => cp.SerialNumberNew, f => f.Random.AlphaNumeric(12))
                .RuleFor(cp => cp.Action, f => f.PickRandom(new[] { ClaimPartAction.Replace.GetClaimPartAction(), ClaimPartAction.Repair.GetClaimPartAction() }))
                .RuleFor(cp => cp.Status, f => f.PickRandom(new[] { ClaimPartStatus.Pending.GetClaimPartStatus(), ClaimPartStatus.Enough.GetClaimPartStatus(), ClaimPartStatus.NotEnough.GetClaimPartStatus() }))
                .RuleFor(cp => cp.Cost, f => f.Finance.Amount(50, 1000));
            var claimParts = claimPartFaker.Generate(joinRecordCount);
            context.ClaimParts.AddRange(claimParts);

            context.SaveChanges(); // Lưu thay đổi CẤP 5 (Cuối cùng)
        }
    }
}