using Bogus;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using System.Text.Json;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        // Small, logic-focused dataset
        public static void SeedDatabase(AppDbContext context)
        {
            // configuration
            const int smallCount = 20;
            Randomizer.Seed = new Random(12345);

            if (context.Organizations.Any())
            {
                return; // DB already seeded
            }

            // We'll gather a small report to aid testing
            var reportLines = new List<string>();

            // === LEVEL 1: base tables ===
            // Organizations: 1 OEM + 2 Service Centers
            var baseOrgFaker = new Faker<Organization>("en")
                .RuleFor(o => o.OrgId, f => f.Database.Random.Guid())
                .RuleFor(o => o.Name, f => f.Company.CompanyName())
                .RuleFor(o => o.Region, f => f.Address.State())
                .RuleFor(o => o.ContactInfo, f => f.Phone.PhoneNumber());

            var oems = baseOrgFaker.Clone()
                .RuleFor(o => o.Type, _ => "OEM")
                .Generate(1);

            var serviceCenters = baseOrgFaker.Clone()
                .RuleFor(o => o.Type, _ => "ServiceCenter")
                .Generate(2);

            var organizations = new List<Organization>();
            organizations.AddRange(oems);
            organizations.AddRange(serviceCenters);

            context.Organizations.AddRange(organizations);
            context.SaveChanges();

            // Employees
            var singleOem = oems.First();
            var baseEmployeeFaker = new Faker<Employee>("en")
                .RuleFor(e => e.UserId, f => f.Database.Random.Guid())
                .RuleFor(e => e.Email, (f, u) => f.Internet.Email(f.Name.FirstName(), f.Name.LastName()))
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.PasswordHash, f => "pass123");

            var employees = new List<Employee>();

            // 1 ADMIN and 1 EVM_STAFF under OEM
            employees.Add(baseEmployeeFaker.Clone()
                .RuleFor(e => e.Role, _ => "ADMIN")
                .RuleFor(e => e.OrgId, _ => singleOem.OrgId)
                .Generate());

            employees.Add(baseEmployeeFaker.Clone()
                .RuleFor(e => e.Role, _ => "EVM_STAFF")
                .RuleFor(e => e.OrgId, _ => singleOem.OrgId)
                .Generate());

            // For each SC: 1 SC_STAFF and 3 SC_TECH
            foreach (var sc in serviceCenters)
            {
                employees.Add(baseEmployeeFaker.Clone()
                    .RuleFor(e => e.Role, _ => "SC_STAFF")
                    .RuleFor(e => e.OrgId, _ => sc.OrgId)
                    .Generate());

                employees.AddRange(baseEmployeeFaker.Clone()
                    .RuleFor(e => e.Role, _ => "SC_TECH")
                    .RuleFor(e => e.OrgId, _ => sc.OrgId)
                    .Generate(3));
            }

            context.Employees.AddRange(employees);

            // Customers
            var customerFaker = new Faker<Customer>("en")
                .RuleFor(c => c.CustomerId, f => f.Database.Random.Guid())
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.OrganizationOrgId, _ => singleOem.OrgId);
            var customers = customerFaker.Generate(10);
            context.Customers.AddRange(customers);

            // Parts: OEM has all models, SC has low stock
            var allModels = PartModel.ModelsByCategory.SelectMany(kvp => kvp.Value).Distinct().ToList();
            var rng = new Random(777);
            var parts = new List<Part>();

            // OEM stock big
            foreach (var model in allModels)
            {
                var category = PartModel.GetCategoryByModel(model) ?? PartCategory.Other.GetPartCategory();
                parts.Add(new Part
                {
                    PartId = Guid.NewGuid(),
                    Model = model,
                    Name = $"{category} - {model}",
                    Category = category,
                    StockQuantity = rng.Next(100, 301),
                    OrgId = singleOem.OrgId
                });
            }

            // SC stocks: 0-10
            foreach (var sc in serviceCenters)
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
                        StockQuantity = rng.Next(0, 11),
                        OrgId = sc.OrgId
                    });
                }
            }

            context.Parts.AddRange(parts);

            // Policies
            var policyFaker = new Faker<WarrantyPolicy>("en")
                .RuleFor(p => p.PolicyId, f => f.Database.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName() + " Warranty")
                .RuleFor(p => p.CoveragePeriodMonths, f => f.PickRandom(new[] { 12, 24, 36 }))
                .RuleFor(p => p.Conditions, f => f.Lorem.Sentence())
                .RuleFor(p => p.OrganizationOrgId, _ => singleOem.OrgId);
            var policies = policyFaker.Generate(4);
            context.WarrantyPolicies.AddRange(policies);

            // Campaigns (2-3)
            var scTechs = employees.Where(e => e.Role == "SC_TECH").ToList();
            var campaignFaker = new Faker<Campaign>("en")
                .RuleFor(c => c.CampaignId, f => f.Database.Random.Guid())
                .RuleFor(c => c.Title, f => f.Company.Bs() + " Campaign")
                .RuleFor(c => c.Type, f => f.PickRandom(new[] { "RECALL", "SERVICE" }))
                .RuleFor(c => c.Description, f => f.Lorem.Sentence())
                .RuleFor(c => c.StartDate, f => f.Date.Past(1))
                .RuleFor(c => c.EndDate, f => f.Date.Soon(90))
                .RuleFor(c => c.Status, f => f.PickRandom(new[] { "DRAFT", "ACTIVE" }))
                .RuleFor(c => c.CreatedBy, f => f.PickRandom(scTechs).UserId)
                .RuleFor(c => c.CreatedAt, f => f.Date.Recent(60))
                .RuleFor(c => c.PartModel, f => f.PickRandom(allModels))
                .RuleFor(c => c.ReplacementPartModel, (f, c) =>
                {
                    var candidates = allModels.Where(m => !string.Equals(m, c.PartModel, StringComparison.OrdinalIgnoreCase)).ToList();
                    return f.PickRandom(candidates);
                });
            var campaigns = campaignFaker.Generate(2);
            context.Campaigns.AddRange(campaigns);

            context.SaveChanges();

            // Record basic campaign info
            foreach (var c in campaigns)
            {
                reportLines.Add($"Campaign: {c.Title} | PartModel: {c.PartModel} | Replacement: {c.ReplacementPartModel}");
            }

            // === LEVEL 2: dependent on base ===
            // Vehicles
            var vehicleFaker = new Faker<Vehicle>("en")
                .RuleFor(v => v.Vin, f => f.Random.AlphaNumeric(17).ToUpper())
                .RuleFor(v => v.Model, f => f.Vehicle.Model())
                .RuleFor(v => v.Year, f => f.Random.Int(2020, 2025))
                .RuleFor(v => v.CustomerId, f => f.PickRandom(customers).CustomerId);
            var vehicles = vehicleFaker.Generate(10);
            context.Vehicles.AddRange(vehicles);

            // Vehicle parts: many parts for some vehicles; each part has serial; same model can appear multiple times
            var vehicleParts = new List<VehiclePart>();
            var rng2 = new Random(999);
            foreach (var v in vehicles)
            {
                // 1-3 distinct models, each with 1-4 quantities (serials)
                var modelsForVehicle = allModels.OrderBy(_ => rng2.Next()).Take(rng2.Next(1, 4)).ToList();
                foreach (var m in modelsForVehicle)
                {
                    var qty = rng2.Next(1, 5);
                    for (int i = 0; i < qty; i++)
                    {
                        var installed = DateTime.UtcNow.AddDays(-rng2.Next(60, 600));
                        vehicleParts.Add(new VehiclePart
                        {
                            VehiclePartId = Guid.NewGuid(),
                            Vin = v.Vin,
                            Model = m,
                            SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                            InstalledDate = installed,
                            UninstalledDate = DateTime.MinValue,
                            Status = VehiclePartStatus.Installed.GetVehiclePartStatus()
                        });
                    }
                }
            }
            context.VehicleParts.AddRange(vehicleParts);

            // Ensure vehicles have campaign PartModel installed so campaign is testable
            foreach (var c in campaigns)
            {
                var model = c.PartModel;
                var ensuredVins = new List<string>();
                // pick 5 vehicles, ensure at least 1-2 serials for the model
                var ensureVehicles = vehicles.OrderBy(_ => rng.Next()).Take(5).ToList();
                foreach (var v in ensureVehicles)
                {
                    var hasModel = vehicleParts.Any(vp => vp.Vin == v.Vin && vp.Model == model && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus());
                    if (!hasModel)
                    {
                        var qty = rng.Next(1, 3);
                        for (int i = 0; i < qty; i++)
                        {
                            vehicleParts.Add(new VehiclePart
                            {
                                VehiclePartId = Guid.NewGuid(),
                                Vin = v.Vin,
                                Model = model!,
                                SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                                InstalledDate = DateTime.UtcNow.AddDays(-rng.Next(30, 120)),
                                UninstalledDate = DateTime.MinValue,
                                Status = VehiclePartStatus.Installed.GetVehiclePartStatus()
                            });
                        }
                    }
                    ensuredVins.Add(v.Vin);
                }
                reportLines.Add($"Campaign Part prepared on VINs (may include pre-existing): {string.Join(", ", ensuredVins)}");
            }
            context.SaveChanges();

            // Part Orders for each SC (ensure items exist)
            var scOrgs = serviceCenters.ToList();
            var scEmployeeIds = employees.Where(e => e.Role == "SC_STAFF" || e.Role == "SC_TECH").Select(e => e.UserId).ToList();
            var partOrders = new List<PartOrder>();
            var partOrderItems = new List<PartOrderItem>();
            foreach (var sc in scOrgs)
            {
                // 2 orders per SC
                for (int i = 0; i < 2; i++)
                {
                    var order = new PartOrder
                    {
                        OrderId = Guid.NewGuid(),
                        ServiceCenterId = sc.OrgId,
                        RequestDate = DateTime.UtcNow.AddDays(-rng.Next(10, 60)),
                        Status = PartOrderStatus.Waiting.GetPartOrderStatus(),
                        CreatedBy = employees.First(e => e.Role == "SC_STAFF" && e.OrgId == sc.OrgId).UserId,
                        ExpectedDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(rng.Next(5, 15)))
                    };

                    // Randomly progress status
                    if (rng.NextDouble() < 0.6)
                    {
                        order.ApprovedDate = DateTime.UtcNow.AddDays(-rng.Next(1, 5));
                        order.Status = PartOrderStatus.Confirm.GetPartOrderStatus();
                    }
                    if (rng.NextDouble() < 0.4)
                    {
                        order.ShippedDate = DateTime.UtcNow.AddDays(-rng.Next(1, 3));
                        order.Status = PartOrderStatus.Delivery.GetPartOrderStatus();
                        order.PartDelivery = DateTime.UtcNow.AddDays(rng.Next(0, 4));
                    }

                    partOrders.Add(order);

                    // 1-3 items
                    var orderModels = allModels.OrderBy(_ => rng.Next()).Take(rng.Next(1, 4)).ToList();
                    foreach (var model in orderModels)
                    {
                        partOrderItems.Add(new PartOrderItem
                        {
                            OrderItemId = Guid.NewGuid(),
                            OrderId = order.OrderId,
                            Model = model,
                            Quantity = rng.Next(1, 6),
                            Remarks = "Test order item"
                        });
                    }
                }
            }
            context.PartOrders.AddRange(partOrders);
            context.PartOrderItems.AddRange(partOrderItems);

            context.SaveChanges();

            // === LEVEL 3: Campaign Vehicles & replacements ===
            var campaignVehicles = new List<CampaignVehicle>();
            foreach (var c in campaigns)
            {
                var model = c.PartModel;
                // find vehicles that have this model installed
                var vehiclesWithModel = vehicles
                    .Where(v => vehicleParts.Any(vp => vp.Vin == v.Vin && vp.Model == model && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus()))
                    .OrderBy(_ => rng.Next())
                    .Take(4)
                    .ToList();

                // Pick 3 vehicles per campaign
                var picked = vehiclesWithModel.Take(3).ToList();
                reportLines.Add($"Campaign Vehicles for {c.Title} ({c.PartModel} -> {c.ReplacementPartModel}): {string.Join(", ", picked.Select(x => x.Vin))}");
                foreach (var v in picked)
                {
                    // Status scenario: waiting, under repair (assigned), repaired
                    var statusPick = rng.Next(0, 3);
                    var cv = new CampaignVehicle
                    {
                        CampaignVehicleId = Guid.NewGuid(),
                        CampaignId = c.CampaignId,
                        Vin = v.Vin,
                        CreatedAt = DateTime.UtcNow.AddDays(-rng.Next(5, 50)),
                        Status = statusPick == 0
                            ? CampaignVehicleStatus.WaitingForUnassignedRepair.GetCampaignVehicleStatus()
                            : statusPick == 1
                                ? CampaignVehicleStatus.UnderRepair.GetCampaignVehicleStatus()
                                : CampaignVehicleStatus.Repaired.GetCampaignVehicleStatus(),
                    };

                    if (cv.Status == CampaignVehicleStatus.Repaired.GetCampaignVehicleStatus())
                    {
                        cv.CompletedAt = DateTime.UtcNow.AddDays(-rng.Next(1, 5));
                    }

                    campaignVehicles.Add(cv);
                }
            }
            context.CampaignVehicles.AddRange(campaignVehicles);
            context.SaveChanges();

            // Assign work orders for CampaignVehicle if UnderRepair
            var campaignUnderRepair = campaignVehicles.Where(x => x.Status == CampaignVehicleStatus.UnderRepair.GetCampaignVehicleStatus()).ToList();
            foreach (var cv in campaignUnderRepair)
            {
                var anySc = scTechs.OrderBy(_ => rng.Next()).First();
                var wo = new WorkOrder
                {
                    WorkOrderId = Guid.NewGuid(),
                    AssignedTo = anySc.UserId,
                    Type = WorkOrderType.Repair.GetWorkOrderType(),
                    Target = WorkOrderTarget.Campaign.GetWorkOrderTarget(),
                    TargetId = cv.CampaignVehicleId,
                    Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                    StartDate = DateTime.UtcNow.AddDays(-rng.Next(1, 7))
                };
                context.WorkOrders.Add(wo);
            }

            // Create replacements for Repaired/Done (and fill NewSerial accordingly) only for the campaign's PartModel, using ReplacementPartModel
            var replacements = new List<CampaignVehicleReplacement>();
            foreach (var cv in campaignVehicles.Where(cv => cv.Status == CampaignVehicleStatus.Repaired.GetCampaignVehicleStatus() || cv.Status == CampaignVehicleStatus.Done.GetCampaignVehicleStatus()))
            {
                var campaign = campaigns.First(c => c.CampaignId == cv.CampaignId);
                var partModel = campaign.PartModel;
                var replacementModel = campaign.ReplacementPartModel;

                var vpInstalled = vehicleParts.Where(vp => vp.Vin == cv.Vin && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus() && vp.Model == partModel).ToList();
                if (!vpInstalled.Any()) continue;

                var toReplace = vpInstalled.OrderBy(_ => rng.Next()).Take(rng.Next(1, Math.Min(3, vpInstalled.Count))).ToList();
                var newSerials = new List<string>();
                var replacedAt = cv.CompletedAt ?? DateTime.UtcNow.AddDays(-rng.Next(1, 10));

                foreach (var oldPart in toReplace)
                {
                    // mark old uninstalled
                    oldPart.Status = VehiclePartStatus.UnInstalled.GetVehiclePartStatus();
                    oldPart.UninstalledDate = replacedAt;

                    // add new with replacement model (different from failed model)
                    var newSerial = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}";
                    var newPart = new VehiclePart
                    {
                        VehiclePartId = Guid.NewGuid(),
                        Vin = cv.Vin,
                        Model = replacementModel!,
                        SerialNumber = newSerial,
                        InstalledDate = replacedAt,
                        UninstalledDate = DateTime.MinValue,
                        Status = VehiclePartStatus.Installed.GetVehiclePartStatus()
                    };
                    context.VehicleParts.Add(newPart);
                    vehicleParts.Add(newPart);

                    replacements.Add(new CampaignVehicleReplacement
                    {
                        CampaignVehicleReplacementId = Guid.NewGuid(),
                        CampaignVehicleId = cv.CampaignVehicleId,
                        OldSerial = oldPart.SerialNumber,
                        NewSerial = newSerial,
                        ReplacedAt = replacedAt
                    });
                    newSerials.Add(newSerial);
                }

                cv.NewSerial = JsonSerializer.Serialize(newSerials, (JsonSerializerOptions?)null);
                cv.CompletedAt = replacedAt;
            }
            if (replacements.Count > 0)
            {
                context.CampaignVehicleReplacements.AddRange(replacements);
            }

            context.SaveChanges();

            // === LEVEL 4: Warranty Claims + related ===
            // Create an active vehicle policy for a demo vehicle
            var demoVehicle = vehicles.First();
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

            // Build some claims with consistent statuses
            var claims = new List<WarrantyClaim>();
            var sc1 = serviceCenters[0];
            var sc2 = serviceCenters[1];
            var sc1Staff = employees.First(e => e.Role == "SC_STAFF" && e.OrgId == sc1.OrgId);
            var sc2Staff = employees.First(e => e.Role == "SC_STAFF" && e.OrgId == sc2.OrgId);
            var evmStaff = employees.First(e => e.Role == "EVM_STAFF");

            // Helper to pick installed part model on a vehicle
            string PickInstalledModel(string vin)
            {
                return vehicleParts.First(vp => vp.Vin == vin && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus()).Model;
            }

            // 2 waiting for unassigned
            foreach (var sc in new[] { sc1, sc2 })
            {
                var veh = vehicles[rng.Next(vehicles.Count)];
                var c = new WarrantyClaim
                {
                    ClaimId = Guid.NewGuid(),
                    Vin = veh.Vin,
                    ServiceCenterId = sc.OrgId,
                    CreatedBy = (sc.OrgId == sc1.OrgId ? sc1Staff.UserId : sc2Staff.UserId),
                    CreatedDate = DateTime.UtcNow.AddDays(-rng.Next(2, 10)),
                    Status = WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus(),
                    Description = "Customer reports issue",
                    failureDesc = "Intermittent error"
                };
                claims.Add(c);

                // add 1 claim part
                context.ClaimParts.Add(new ClaimPart
                {
                    ClaimPartId = Guid.NewGuid(),
                    ClaimId = c.ClaimId,
                    Model = PickInstalledModel(veh.Vin),
                    SerialNumberOld = vehicleParts.First(vp => vp.Vin == veh.Vin).SerialNumber,
                    SerialNumberNew = null,
                    Action = ClaimPartAction.Replace.GetClaimPartAction(),
                    Status = ClaimPartStatus.Pending.GetClaimPartStatus(),
                    Cost = 0
                });
            }

            // 2 under inspection => must have inspection WO assigned
            foreach (var sc in new[] { sc1, sc2 })
            {
                var veh = vehicles[rng.Next(vehicles.Count)];
                var c = new WarrantyClaim
                {
                    ClaimId = Guid.NewGuid(),
                    Vin = veh.Vin,
                    ServiceCenterId = sc.OrgId,
                    CreatedBy = (sc.OrgId == sc1.OrgId ? sc1Staff.UserId : sc2Staff.UserId),
                    CreatedDate = DateTime.UtcNow.AddDays(-rng.Next(5, 12)),
                    Status = WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus(),
                    Description = "Noise during acceleration",
                    failureDesc = "Noise from motor"
                };
                claims.Add(c);
                var tech = employees.First(e => e.Role == "SC_TECH" && e.OrgId == sc.OrgId);
                context.WorkOrders.Add(new WorkOrder
                {
                    WorkOrderId = Guid.NewGuid(),
                    AssignedTo = tech.UserId,
                    Type = WorkOrderType.Inspection.GetWorkOrderType(),
                    Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                    TargetId = c.ClaimId,
                    Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                    StartDate = DateTime.UtcNow.AddDays(-1)
                });
            }

            // 2 approved => confirmed by EVM staff
            foreach (var sc in new[] { sc1, sc2 })
            {
                var veh = vehicles[rng.Next(vehicles.Count)];
                var c = new WarrantyClaim
                {
                    ClaimId = Guid.NewGuid(),
                    Vin = veh.Vin,
                    ServiceCenterId = sc.OrgId,
                    CreatedBy = (sc.OrgId == sc1.OrgId ? sc1Staff.UserId : sc2Staff.UserId),
                    CreatedDate = DateTime.UtcNow.AddDays(-rng.Next(7, 15)),
                    Status = WarrantyClaimStatus.Approved.GetWarrantyClaimStatus(),
                    Description = "Approved repair",
                    ConfirmBy = evmStaff.UserId,
                    ConfirmDate = DateTime.UtcNow.AddDays(-rng.Next(1, 5)),
                    VehicleWarrantyId = demoPolicy.VehicleWarrantyId,
                    failureDesc = "BMS reset"
                };
                claims.Add(c);
                // claim parts enough
                context.ClaimParts.Add(new ClaimPart
                {
                    ClaimPartId = Guid.NewGuid(),
                    ClaimId = c.ClaimId,
                    Model = PickInstalledModel(veh.Vin),
                    SerialNumberOld = vehicleParts.First(vp => vp.Vin == veh.Vin).SerialNumber,
                    SerialNumberNew = null,
                    Action = ClaimPartAction.Replace.GetClaimPartAction(),
                    Status = ClaimPartStatus.Enough.GetClaimPartStatus(),
                    Cost = 0
                });
            }

            // 2 under repair => must have repair WO assigned
            foreach (var sc in new[] { sc1, sc2 })
            {
                var veh = vehicles[rng.Next(vehicles.Count)];
                var c = new WarrantyClaim
                {
                    ClaimId = Guid.NewGuid(),
                    Vin = veh.Vin,
                    ServiceCenterId = sc.OrgId,
                    CreatedBy = (sc.OrgId == sc1.OrgId ? sc1Staff.UserId : sc2Staff.UserId),
                    CreatedDate = DateTime.UtcNow.AddDays(-rng.Next(10, 20)),
                    Status = WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus(),
                    Description = "Repair in progress",
                    ConfirmBy = evmStaff.UserId,
                    ConfirmDate = DateTime.UtcNow.AddDays(-rng.Next(3, 7)),
                    VehicleWarrantyId = demoPolicy.VehicleWarrantyId,
                    failureDesc = "Controller fault"
                };
                claims.Add(c);
                var tech = employees.First(e => e.Role == "SC_TECH" && e.OrgId == sc.OrgId);
                context.WorkOrders.Add(new WorkOrder
                {
                    WorkOrderId = Guid.NewGuid(),
                    AssignedTo = tech.UserId,
                    Type = WorkOrderType.Repair.GetWorkOrderType(),
                    Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                    TargetId = c.ClaimId,
                    Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                    StartDate = DateTime.UtcNow.AddDays(-2)
                });
            }

            context.WarrantyClaims.AddRange(claims);

            // Attachments/back-claims a few
            var attachments = new Faker<ClaimAttachment>("en")
                .RuleFor(a => a.AttachmentId, f => f.Database.Random.Guid().ToString())
                .RuleFor(a => a.ClaimId, f => f.PickRandom(claims).ClaimId)
                .RuleFor(a => a.URL, f => f.Image.PicsumUrl())
                .RuleFor(a => a.UploadedBy, f => f.PickRandom(scEmployeeIds))
                .Generate(6);
            context.ClaimAttachments.AddRange(attachments);

            var backClaims = new Faker<BackWarrantyClaim>("en")
                .RuleFor(b => b.WarrantyClaimId, f => f.PickRandom(claims).ClaimId)
                .RuleFor(b => b.CreatedDate, f => f.Date.Recent(15))
                .RuleFor(b => b.Description, f => f.Lorem.Sentence(6))
                .RuleFor(b => b.CreatedByEmployeeId, f => f.PickRandom(scEmployeeIds))
                .Generate(6);
            context.BackWarrantyClaims.AddRange(backClaims);

            // A few appointments
            var vinToCustomer = vehicles.ToDictionary(v => v.Vin, v => v.CustomerId);
            var appointments = new Faker<Appointment>("en")
                .RuleFor(a => a.AppointmentId, f => f.Database.Random.Guid())
                .RuleFor(a => a.AppointmentType, f => f.PickRandom(new[] { "WARRANTY", "CAMPAIGN" }))
                .RuleFor(a => a.Vin, f => f.PickRandom(vehicles).Vin)
                .RuleFor(a => a.CustomerId, (f, a) => vinToCustomer[a.Vin])
                .RuleFor(a => a.CampaignVehicleId, (f, a) =>
                {
                    if (a.AppointmentType == "CAMPAIGN")
                    {
                        return (Guid?)context.CampaignVehicles.OrderBy(_ => Guid.NewGuid()).Select(x => x.CampaignVehicleId).FirstOrDefault();
                    }
                    return (Guid?)null;
                })
                .RuleFor(a => a.ServiceCenterId, f => f.PickRandom(scOrgs).OrgId)
                .RuleFor(a => a.AppointmentDate, f => DateOnly.FromDateTime(f.Date.Soon(30)))
                .RuleFor(a => a.Status, f => f.PickRandom(new[] { "SCHEDULED", "CHECKED_IN", "CANCELLED" }))
                .RuleFor(a => a.CreatedAt, f => f.Date.Recent(30))
                .RuleFor(a => a.Note, f => f.Random.Bool(0.3f) ? f.Lorem.Sentence() : null)
                .Generate(8);
            context.Appointments.AddRange(appointments);

            context.SaveChanges();

            // Emit report lines to console and file to help testers
            try
            {
                Console.WriteLine("==== Seed Report (for quick testing) ====");
                foreach (var line in reportLines)
                {
                    Console.WriteLine(line);
                }
                var path = Path.Combine(AppContext.BaseDirectory, "SeedReport.txt");
                File.WriteAllLines(path, reportLines);
                Console.WriteLine($"Seed report written to: {path}");
            }
            catch { /* ignore diagnostics write failures */ }
        }
    }
}