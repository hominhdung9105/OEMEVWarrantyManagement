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

            // 2. Tạo 2 OEM
            var oems = baseOrgFaker
                .RuleFor(o => o.Type, "OEM") // Gán cứng Type là OEM
                .Generate(2);               // Tạo 2 bản ghi

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

            context.SaveChanges(); // Lưu cả 5 organizations

            // --- CẤP 2: PHỤ THUỘC VÀO CẤP 1 ---

            // --- SỬA LOGIC TẠO EMPLOYEE ---
            // 1. Lấy danh sách OEM và ServiceCenter đã tạo
            var oemOrgs = organizations.Where(o => o.Type == "OEM").ToList();
            var scOrgs = organizations.Where(o => o.Type == "ServiceCenter").ToList();

            // 2. Tạo Faker cơ bản cho Employee
            var baseEmployeeFaker = new Faker<Employee>("en")
                .RuleFor(e => e.UserId, f => f.Database.Random.Guid())
                .RuleFor(e => e.Email, (f, u) => f.Internet.Email(f.Name.FirstName(), f.Name.LastName()))
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.PasswordHash, f => "pass123");

            // 3. Tạo danh sách tổng hợp employee
            var employees = new List<Employee>();

            // 4. Tạo 1 ADMIN (gán vào OEM đầu tiên)
            if (oemOrgs.Any())
            {
                var admin = baseEmployeeFaker
                    .RuleFor(e => e.Role, "ADMIN")
                    .RuleFor(e => e.OrgId, oemOrgs.First().OrgId) // Gán vào OEM đầu tiên
                    .Generate();
                employees.Add(admin);
            }

            // 5. Tạo 4 EVM_STAFF (2 cho mỗi OEM)
            foreach (var oem in oemOrgs)
            {
                var evmStaff = baseEmployeeFaker
                    .RuleFor(e => e.Role, "EVM_STAFF")
                    .RuleFor(e => e.OrgId, oem.OrgId) // Gán vào OEM hiện tại
                    .Generate(2); // Tạo 2 người
                employees.AddRange(evmStaff);
            }

            // 6. Tạo 6 SC_STAFF và 15 SC_TECH (chia đều cho 3 ServiceCenter)
            foreach (var sc in scOrgs)
            {
                // Tạo 2 SC_STAFF cho Service Center này
                var scStaff = baseEmployeeFaker
                        .RuleFor(e => e.Role, "SC_STAFF")
                        .RuleFor(e => e.OrgId, sc.OrgId) // Gán vào SC hiện tại
                        .Generate(2);
                employees.AddRange(scStaff);

                // Tạo 5 SC_TECH cho Service Center này
                var scTech = baseEmployeeFaker
                        .RuleFor(e => e.Role, "SC_TECH")
                        .RuleFor(e => e.OrgId, sc.OrgId) // Gán vào SC hiện tại
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

            // --- SỬA LOGIC TẠO PART: Mỗi Org không có 2 Model trùng nhau ---
            var allModels = PartModel.ModelsByCategory.SelectMany(kvp => kvp.Value).Distinct().ToList();
            var rng = new Random();
            var parts = new List<Part>();

            // Số part cho mỗi tổ chức (xáp xỉ)
            var partsPerOrg = Math.Max(10, primaryRecordCount / Math.Max(1, organizations.Count));

            foreach (var org in organizations)
            {
                // Chọn ngẫu nhiên các model không trùng nhau cho org này
                var modelsForOrg = allModels
                    .OrderBy(_ => rng.Next())
                    .Take(Math.Min(partsPerOrg, allModels.Count))
                    .ToList();

                foreach (var model in modelsForOrg)
                {
                    var category = PartModel.GetCategoryByModel(model) ?? PartCategory.Other.GetPartCategory();
                    parts.Add(new Part
                    {
                        PartId = Guid.NewGuid(),
                        Model = model,
                        Name = $"{category} - {model}",
                        Category = category,
                        StockQuantity = rng.Next(50, 501),
                        OrgId = org.OrgId
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
                .RuleFor(c => c.Name, f => f.Company.Bs() + " Campaign")
                .RuleFor(c => c.Type, f => f.PickRandom(new[] { "Recall", "Service", "Update" }))
                .RuleFor(c => c.Description, f => f.Lorem.Sentence())
                .RuleFor(c => c.StartDate, f => f.Date.Past(1))
                .RuleFor(c => c.EndDate, f => f.Date.Future(1))
                .RuleFor(c => c.Status, f => "Active")
                .RuleFor(c => c.OrganizationOrgId, (f, u) => f.PickRandom(organizations).OrgId);
            var campaigns = campaignFaker.Generate(primaryRecordCount);
            context.Campaigns.AddRange(campaigns);

            // --- WORKORDER ĐÃ BỊ XÓA KHỎI ĐÂY ---

            var partOrderFaker = new Faker<PartOrder>("en")
                .RuleFor(p => p.OrderId, f => f.Database.Random.Guid())
                .RuleFor(p => p.ServiceCenterId, (f, u) => f.PickRandom(organizations).OrgId)
                .RuleFor(p => p.RequestDate, f => f.Date.Past(1))
                .RuleFor(p => p.ApprovedDate, (f, p) => f.Random.Bool(0.7f) ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(p => p.ShippedDate, (f, p) => p.ApprovedDate.HasValue ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(p => p.ExpectedDate, (f, p) => p.RequestDate.AddDays(f.Random.Int(5, 15)))
                .RuleFor(p => p.PartDelivery, (f, p) => p.ShippedDate.HasValue ? p.ExpectedDate?.AddDays(f.Random.Int(-1, 3)) : (DateTime?)null)
                .RuleFor(p => p.Status, f => f.PickRandom(new[] { "Pending", "Waiting", "Deliverd" }))
                .RuleFor(p => p.CreatedBy, (f, u) => f.PickRandom(employees).UserId);
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
                .RuleFor(cv => cv.NotifiedDate, f => f.Date.Past(1))
                .RuleFor(cv => cv.HandledDate, (f, cv) => f.Random.Bool(0.5f) ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(cv => cv.Status, (f, cv) => cv.HandledDate.HasValue ? "Completed" : "Notified");
            var campaignVehicles = campaignVehicleFaker.Generate(joinRecordCount);
            context.CampaignVehicles.AddRange(campaignVehicles);

            var vehiclePartFaker = new Faker<VehiclePart>("en")
                 .RuleFor(vp => vp.VehiclePartId, f => f.Database.Random.Guid())
                 .RuleFor(vp => vp.Vin, (f, u) => f.PickRandom(vehicles).Vin)
                 // Chọn một Part ngẫu nhiên từ danh sách đã tạo
                 .FinishWith((f, vp) => {
                     var selectedPart = f.PickRandom(parts); // Lấy một Part ngẫu nhiên
                     vp.Model = selectedPart.Model;          // Gán Model từ Part đó
                 })
                 .RuleFor(vp => vp.SerialNumber, f => f.Random.AlphaNumeric(12))
                 .RuleFor(vp => vp.InstalledDate, f => f.Date.Past(2))
                 .RuleFor(vp => vp.UninstalledDate, (f, vp) => f.Date.Between(vp.InstalledDate.AddDays(1), vp.InstalledDate.AddYears(5)))
                 .RuleFor(vp => vp.Status, (f, vp) => vp.UninstalledDate > DateTime.UtcNow ? "Active" : "Uninstalled");

            var vehicleParts = vehiclePartFaker.Generate(joinRecordCount);
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
                .RuleFor(c => c.ServiceCenterId, (f, u) => f.PickRandom(organizations.Where(o => o.Type == "ServiceCenter")).OrgId)
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

            context.SaveChanges(); // Lưu thay đổi CẤP 4

            // --- CẤP 5: PHỤ THUỘC VÀO CẤP 4 ---

            // --- WORKORDER ĐÃ ĐƯỢC DI CHUYỂN XUỐNG ĐÂY ---
            var workOrderFaker = new Faker<WorkOrder>("en")
                .RuleFor(w => w.WorkOrderId, f => f.Database.Random.Guid())
                .RuleFor(w => w.AssignedTo, (f, u) => f.PickRandom(employees).UserId)
                .RuleFor(w => w.Type, f => f.PickRandom(new[] { "Repair", "Inspection" })) // Giữ nguyên Type của bạn
                .RuleFor(w => w.Status, f => f.PickRandom(new[] { "in progress", "completed" }))
                .RuleFor(w => w.StartDate, f => f.Date.Past(1))
                .RuleFor(w => w.EndDate, (f, w) => w.Status == "Completed" ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(w => w.Notes, f => f.Lorem.Sentence())
                // --- SỬA LOGIC TARGET ID ---
                .RuleFor(w => w.Target, f => f.PickRandom(new[] { "Campaign", "Warranty" })) // Giữ nguyên Target của bạn
                .RuleFor(w => w.TargetId, (f, w) =>
                {
                    // Nếu Target là "Claim", chọn 1 ClaimId ngẫu nhiên
                    if (w.Target == "Claim")
                    {
                        return f.PickRandom(claims).ClaimId; // 'claims' từ Cấp 4
                    }
                    // Ngược lại, Target là "Campaign", chọn 1 CampaignId ngẫu nhiên
                    return f.PickRandom(campaigns).CampaignId; // 'campaigns' từ Cấp 2
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
                .RuleFor(cp => cp.Action, f => f.PickRandom(new[] { "Replace", "Repair" }))
                .RuleFor(cp => cp.Status, f => f.PickRandom(new[] { "Pending", "Approved", "Shipped" }))
                .RuleFor(cp => cp.Cost, f => f.Finance.Amount(50, 1000));
            var claimParts = claimPartFaker.Generate(joinRecordCount);
            context.ClaimParts.AddRange(claimParts);

            context.SaveChanges(); // Lưu thay đổi CẤP 5 (Cuối cùng)
        }
    }
}