// OEMEVWarrantyManagement.Infrastructure.Persistence/DataSeeder.cs

namespace OEMEVWarrantyManagement.Infrastructure.Persistence
{
    using Bogus;
    using Microsoft.EntityFrameworkCore;
    using OEMEVWarrantyManagement.Domain.Entities; // <-- Giữ nguyên using của bạn
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class DataSeeder
    {
        public static void SeedDatabase(AppDbContext context)
        {
            // --- CÀI ĐẶT ---
            const int primaryRecordCount = 100;
            const int joinRecordCount = 300;
            Randomizer.Seed = new Random(8675309);

            if (context.Organizations.Any())
            {
                return; // DB đã có dữ liệu
            }

            // --- CẤP 1: BẢNG KHÔNG PHỤ THUỘC ---
            var orgFaker = new Faker<Organization>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(o => o.OrgId, f => f.Database.Random.Guid())
                .RuleFor(o => o.Name, f => f.Company.CompanyName())
                .RuleFor(o => o.Type, f => f.PickRandom(new[] { "OEM", "ServiceCenter", "Supplier" }))
                .RuleFor(o => o.Region, f => f.Address.State())
                .RuleFor(o => o.ContactInfo, f => f.Phone.PhoneNumber());

            var organizations = orgFaker.Generate(primaryRecordCount);
            context.Organizations.AddRange(organizations);
            context.SaveChanges();

            // --- CẤP 2: PHỤ THUỘC VÀO CẤP 1 ---
            var employeeFaker = new Faker<Employee>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(e => e.UserId, f => f.Database.Random.Guid())
                .RuleFor(e => e.Email, (f, u) => f.Internet.Email(f.Name.FirstName(), f.Name.LastName()))
                // --- SỬA PASSWORD ---
                // Đây là hash .NET Identity V3 hợp lệ cho "pass123"
                .RuleFor(e => e.PasswordHash, f => "AQAAAAIAAYagAAAAEEfWv12/aVIy/ncy39WJjA/Oq9hBIvA9sbeDxfWxtTR39sD/TjD+fG5qjGhb/Nn1Yg==")
                .RuleFor(e => e.Role, f => f.PickRandom(new[] { "Admin", "Manager", "Technician" }))
                .RuleFor(e => e.OrgId, (f, u) => f.PickRandom(organizations).OrgId)
                .RuleFor(e => e.RefreshToken, f => f.Random.AlphaNumeric(32))
                .RuleFor(e => e.RefreshTokenExpiryTime, f => DateTime.UtcNow.AddDays(7));
            var employees = employeeFaker.Generate(primaryRecordCount);
            context.Employees.AddRange(employees);

            var customerFaker = new Faker<Customer>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(c => c.CustomerId, f => f.Database.Random.Guid())
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.OrganizationOrgId, (f, u) => f.PickRandom(organizations).OrgId);
            var customers = customerFaker.Generate(primaryRecordCount);
            context.Customers.AddRange(customers);

            var partFaker = new Faker<Part>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(p => p.PartId, f => f.Database.Random.Guid())
                .RuleFor(p => p.Model, f => f.Vehicle.Model() + "-" + f.Random.AlphaNumeric(5))
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Category, f => f.PickRandom(new[] { "Battery", "Motor", "Inverter", "Body", "Sensor" }))
                .RuleFor(p => p.StockQuantity, f => f.Random.Number(50, 500))
                .RuleFor(p => p.OrgId, (f, u) => f.PickRandom(organizations).OrgId);
            var parts = partFaker.Generate(primaryRecordCount);
            context.Parts.AddRange(parts);

            var policyFaker = new Faker<WarrantyPolicy>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(p => p.PolicyId, f => f.Database.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName() + " Warranty")
                .RuleFor(p => p.CoveragePeriodMonths, f => f.PickRandom(new[] { 12, 24, 36, 48 }))
                .RuleFor(p => p.Conditions, f => f.Lorem.Paragraph())
                .RuleFor(p => p.OrganizationOrgId, (f, u) => f.PickRandom(organizations).OrgId);
            var policies = policyFaker.Generate(primaryRecordCount);
            context.WarrantyPolicies.AddRange(policies);

            var campaignFaker = new Faker<Campaign>("en") // <-- SỬA TIẾNG ANH
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

            var workOrderFaker = new Faker<WorkOrder>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(w => w.WorkOrderId, f => f.Database.Random.Guid())
                .RuleFor(w => w.AssignedTo, (f, u) => f.PickRandom(employees).UserId)
                .RuleFor(w => w.Type, f => f.PickRandom(new[] { "Repair", "Maintenance", "Inspection" }))
                .RuleFor(w => w.Target, f => f.PickRandom(new[] { "Vehicle", "Claim" }))
                .RuleFor(w => w.TargetId, f => f.Database.Random.Guid())
                .RuleFor(w => w.Status, f => f.PickRandom(new[] { "Pending", "InProgress", "Completed" }))
                .RuleFor(w => w.StartDate, f => f.Date.Past(1))
                .RuleFor(w => w.EndDate, (f, w) => w.Status == "Completed" ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(w => w.Notes, f => f.Lorem.Sentence());
            var workOrders = workOrderFaker.Generate(primaryRecordCount);
            context.WorkOrders.AddRange(workOrders);

            var partOrderFaker = new Faker<PartOrder>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(p => p.OrderId, f => f.Database.Random.Guid())
                .RuleFor(p => p.ServiceCenterId, (f, u) => f.PickRandom(organizations).OrgId)
                .RuleFor(p => p.RequestDate, f => f.Date.Past(1))
                .RuleFor(p => p.ApprovedDate, (f, p) => f.Random.Bool(0.7f) ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(p => p.ShippedDate, (f, p) => p.ApprovedDate.HasValue ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(p => p.Status, f => f.PickRandom(new[] { "Pending", "Approved", "Shipped" }))
                .RuleFor(p => p.CreatedBy, (f, u) => f.PickRandom(employees).UserId);
            var partOrders = partOrderFaker.Generate(primaryRecordCount);
            context.PartOrders.AddRange(partOrders);

            context.SaveChanges(); // Lưu thay đổi CẤP 2

            // --- CẤP 3: PHỤ THUỘC VÀO CẤP 2 ---
            var vehicleFaker = new Faker<Vehicle>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(v => v.Vin, f => f.Random.AlphaNumeric(17).ToUpper())
                .RuleFor(v => v.Model, f => f.Vehicle.Model())
                .RuleFor(v => v.Year, f => f.Random.Int(2020, 2025))
                .RuleFor(v => v.CustomerId, (f, u) => f.PickRandom(customers).CustomerId);
            var vehicles = vehicleFaker.Generate(primaryRecordCount);
            context.Vehicles.AddRange(vehicles);

            var campaignTargetFaker = new Faker<CampaignTarget>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(ct => ct.CampaignTargetId, f => f.Database.Random.Guid())
                .RuleFor(ct => ct.CampaignId, (f, u) => f.PickRandom(campaigns).CampaignId)
                .RuleFor(ct => ct.TargetType, f => f.PickRandom(new[] { "Model", "YearRange", "VIN" }))
                .RuleFor(ct => ct.TargetRefId, f => f.Database.Random.Guid())
                .RuleFor(ct => ct.YearFrom, (f, ct) => ct.TargetType == "YearRange" ? f.Random.Int(2018, 2020) : (int?)null)
                .RuleFor(ct => ct.YearTo, (f, ct) => ct.TargetType == "YearRange" ? f.Random.Int(2021, 2023) : (int?)null);
            var campaignTargets = campaignTargetFaker.Generate(joinRecordCount);
            context.CampaignTargets.AddRange(campaignTargets);

            var partOrderItemFaker = new Faker<PartOrderItem>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(poi => poi.OrderItemId, f => f.Database.Random.Guid())
                .RuleFor(poi => poi.OrderId, (f, u) => f.PickRandom(partOrders).OrderId)
                .RuleFor(poi => poi.PartId, (f, u) => f.PickRandom(parts).PartId)
                .RuleFor(poi => poi.Quantity, f => f.Random.Int(1, 10))
                .RuleFor(poi => poi.Remarks, f => f.Lorem.Sentence());
            var partOrderItems = partOrderItemFaker.Generate(joinRecordCount);
            context.PartOrderItems.AddRange(partOrderItems);

            context.SaveChanges(); // Lưu thay đổi CẤP 3

            // --- CẤP 4: PHỤ THUỘC VÀO CẤP 3 ---
            var campaignVehicleFaker = new Faker<CampaignVehicle>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(cv => cv.CampaignVehicleId, f => f.Database.Random.Guid())
                .RuleFor(cv => cv.CampaignId, (f, u) => f.PickRandom(campaigns).CampaignId)
                .RuleFor(cv => cv.Vin, (f, u) => f.PickRandom(vehicles).Vin)
                .RuleFor(cv => cv.NotifiedDate, f => f.Date.Past(1))
                .RuleFor(cv => cv.HandledDate, (f, cv) => f.Random.Bool(0.5f) ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(cv => cv.Status, (f, cv) => cv.HandledDate.HasValue ? "Completed" : "Notified");
            var campaignVehicles = campaignVehicleFaker.Generate(joinRecordCount);
            context.CampaignVehicles.AddRange(campaignVehicles);

            var vehiclePartFaker = new Faker<VehiclePart>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(vp => vp.VehiclePartId, f => f.Database.Random.Guid())
                .RuleFor(vp => vp.Vin, (f, u) => f.PickRandom(vehicles).Vin)
                .RuleFor(vp => vp.Model, f => f.Vehicle.Model())
                .RuleFor(vp => vp.SerialNumber, f => f.Random.AlphaNumeric(12))
                .RuleFor(vp => vp.InstalledDate, f => f.Date.Past(2))
                .RuleFor(vp => vp.UninstalledDate, (f, vp) => f.Date.Between(vp.InstalledDate.AddDays(1), vp.InstalledDate.AddYears(5)))
                .RuleFor(vp => vp.Status, (f, vp) => vp.UninstalledDate > DateTime.UtcNow ? "Active" : "Uninstalled")
                .RuleFor(vp => vp.PartId, (f, u) => f.PickRandom(parts).PartId);
            var vehicleParts = vehiclePartFaker.Generate(joinRecordCount);
            context.VehicleParts.AddRange(vehicleParts);

            var vehiclePolicyFaker = new Faker<VehicleWarrantyPolicy>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(vp => vp.VehicleWarrantyId, f => f.Database.Random.Guid())
                .RuleFor(vp => vp.Vin, (f, u) => f.PickRandom(vehicles).Vin)
                .RuleFor(vp => vp.PolicyId, (f, u) => f.PickRandom(policies).PolicyId)
                .RuleFor(vp => vp.StartDate, f => f.Date.Past(2))
                .RuleFor(vp => vp.EndDate, (f, vp) => vp.StartDate.AddMonths(f.PickRandom(policies).CoveragePeriodMonths))
                .RuleFor(vp => vp.Status, f => "Active");
            var vehiclePolicies = vehiclePolicyFaker.Generate(joinRecordCount);
            context.VehicleWarrantyPolicies.AddRange(vehiclePolicies);

            var claimFaker = new Faker<WarrantyClaim>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(c => c.ClaimId, f => f.Database.Random.Guid())
                .RuleFor(c => c.Vin, (f, u) => f.PickRandom(vehicles).Vin)
                .RuleFor(c => c.ServiceCenterId, (f, u) => f.PickRandom(organizations.Where(o => o.Type == "ServiceCenter")).OrgId)
                .RuleFor(c => c.CreatedBy, (f, u) => f.PickRandom(employees).UserId)
                .RuleFor(c => c.CreatedDate, f => f.Date.Past(1))
                .RuleFor(c => c.Status, f => f.PickRandom(new[] { "Submitted", "Approved", "Rejected", "Processing" }))
                .RuleFor(c => c.Description, f => f.Lorem.Paragraph())
                .RuleFor(c => c.ConfirmBy, (f, c) => c.Status == "Approved" || c.Status == "Rejected" ? f.PickRandom(employees).UserId : (Guid?)null)
                .RuleFor(c => c.ConfirmDate, (f, c) => c.ConfirmBy.HasValue ? f.Date.Recent() : (DateTime?)null)
                .RuleFor(c => c.PolicyId, (f, u) => f.PickRandom(policies).PolicyId)
                .RuleFor(c => c.failureDesc, f => f.Lorem.Sentence());
            var claims = claimFaker.Generate(primaryRecordCount);
            context.WarrantyClaims.AddRange(claims);

            context.SaveChanges(); // Lưu thay đổi CẤP 4

            // --- CẤP 5: PHỤ THUỘC VÀO CẤP 4 ---
            var backClaimFaker = new Faker<BackWarrantyClaim>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(b => b.WarrantyClaimId, (f, u) => f.PickRandom(claims).ClaimId)
                .RuleFor(b => b.CreatedDate, f => f.Date.Recent())
                .RuleFor(b => b.Description, f => f.Lorem.Sentence(10))
                .RuleFor(b => b.CreatedByEmployeeId, (f, u) => f.PickRandom(employees).UserId);
            var backClaims = backClaimFaker.Generate(joinRecordCount)
                                           .GroupBy(c => new { c.WarrantyClaimId, c.CreatedDate })
                                           .Select(g => g.First())
                                           .ToList();
            context.BackWarrantyClaims.AddRange(backClaims);

            var attachmentFaker = new Faker<ClaimAttachment>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(a => a.AttachmentId, f => f.Database.Random.Guid().ToString())
                .RuleFor(a => a.ClaimId, (f, u) => f.PickRandom(claims).ClaimId)
                .RuleFor(a => a.URL, f => f.Image.PicsumUrl())
                .RuleFor(a => a.UploadedBy, (f, u) => f.PickRandom(employees).UserId);
            var attachments = attachmentFaker.Generate(joinRecordCount);
            context.ClaimAttachments.AddRange(attachments);

            var claimPartFaker = new Faker<ClaimPart>("en") // <-- SỬA TIẾNG ANH
                .RuleFor(cp => cp.ClaimPartId, f => f.Database.Random.Guid())
                .RuleFor(cp => cp.ClaimId, (f, u) => f.PickRandom(claims).ClaimId)
                .RuleFor(cp => cp.Model, f => f.Vehicle.Model())
                .RuleFor(cp => cp.SerialNumberOld, f => f.Random.AlphaNumeric(12))
                .RuleFor(cp => cp.SerialNumberNew, f => f.Random.AlphaNumeric(12))
                .RuleFor(cp => cp.Action, f => f.PickRandom(new[] { "Replace", "Repair" }))
                .RuleFor(cp => cp.Status, f => f.PickRandom(new[] { "Pending", "Approved", "Shipped" }))
                .RuleFor(cp => cp.Cost, f => f.Finance.Amount(50, 1000))
                .RuleFor(cp => cp.PartId, (f, u) => f.PickRandom(parts).PartId);
            var claimParts = claimPartFaker.Generate(joinRecordCount);
            context.ClaimParts.AddRange(claimParts);

            context.SaveChanges(); // Lưu thay đổi CẤP 5 (Cuối cùng)
        }
    }
}