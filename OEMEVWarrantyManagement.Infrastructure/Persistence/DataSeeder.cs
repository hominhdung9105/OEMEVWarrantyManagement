using Bogus;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        // Larger dataset base count for most tables
        public static void SeedDatabase(AppDbContext context)
        {
            // configuration
            const int recordCount = 200;          // keep other entities at 200
            const int customerCount = 50;         // requested change
            const int vehicleCount = 150;          // updated per request
            Randomizer.Seed = new Random(12345);

            if (context.Organizations.Any())
            {
                return; // DB already seeded
            }

            // We'll gather a small report to aid testing
            var reportLines = new List<string>();

            // === LEVEL 1: base tables ===
            // Organizations: 1 OEM + 3 Service Centers (changed from 2 to 3)
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
                .Generate(3); // Changed from 2 to 3

            var organizations = new List<Organization>();
            organizations.AddRange(oems);
            organizations.AddRange(serviceCenters);

            context.Organizations.AddRange(organizations);
            context.SaveChanges();

            // Employees: 1 ADMIN, 2 EVM_STAFF, 6 SC_STAFF (2 per org), 15 SC_TECH (5 per org)
            var singleOem = oems.First();
            
            // Create PasswordHasher to properly hash passwords
            var passwordHasher = new PasswordHasher<Employee>();
            
            var baseEmployeeFaker = new Faker<Employee>("en")
                .RuleFor(e => e.UserId, f => f.Database.Random.Guid())
                .RuleFor(e => e.Email, (f, u) => f.Internet.Email(f.Name.FirstName(), f.Name.LastName()))
                .RuleFor(e => e.Name, f => f.Name.FullName())
                .RuleFor(e => e.PasswordHash, (f, emp) => 
                {
                    // Hash the password "pass123" for each employee
                    return passwordHasher.HashPassword(emp, "P@ssW0rd");
                });

            var employees = new List<Employee>();

            // 1 ADMIN under OEM
            employees.Add(baseEmployeeFaker.Clone()
                .RuleFor(e => e.Role, _ => "ADMIN")
                .RuleFor(e => e.OrgId, _ => singleOem.OrgId)
                .Generate());

            // 2 EVM_STAFF under OEM
            employees.AddRange(baseEmployeeFaker.Clone()
                .RuleFor(e => e.Role, _ => "EVM_STAFF")
                .RuleFor(e => e.OrgId, _ => singleOem.OrgId)
                .Generate(2));

            // For each SC: 2 SC_STAFF and 5 SC_TECH
            foreach (var sc in serviceCenters)
            {
                employees.AddRange(baseEmployeeFaker.Clone()
                    .RuleFor(e => e.Role, _ => "SC_STAFF")
                    .RuleFor(e => e.OrgId, _ => sc.OrgId)
                    .Generate(2));

                employees.AddRange(baseEmployeeFaker.Clone()
                    .RuleFor(e => e.Role, _ => "SC_TECH")
                    .RuleFor(e => e.OrgId, _ => sc.OrgId)
                    .Generate(5));
            }

            context.Employees.AddRange(employees);

            // Customers - 50 records (updated)
            var customerFaker = new Faker<Customer>("en")
                .RuleFor(c => c.CustomerId, f => f.Database.Random.Guid())
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.OrganizationOrgId, _ => singleOem.OrgId);
            var customers = customerFaker.Generate(customerCount);
            context.Customers.AddRange(customers);

            // Parts: OEM has all models, SCs have varying stock - based on all models
            // NOTE: There's a unique index on (OrgId, Model), so each org can only have ONE part per model
            var allModels = PartModel.ModelsByCategory.SelectMany(kvp => kvp.Value).Distinct().ToList();
            var rng = new Random(777);
            var parts = new List<Part>();

            // OEM gets all unique models (one per model) with high stock
            foreach (var model in allModels)
            {
                var category = PartModel.GetCategoryByModel(model) ?? "Unknown";
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

            // Each SC gets all unique models (one per model) with low stock
            foreach (var sc in serviceCenters)
            {
                foreach (var model in allModels)
                {
                    var category = PartModel.GetCategoryByModel(model) ?? "Unknown";
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

            // Policies - keep 200
            var policyFaker = new Faker<WarrantyPolicy>("en")
                .RuleFor(p => p.PolicyId, f => f.Database.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName() + " Warranty")
                .RuleFor(p => p.CoveragePeriodMonths, f => f.PickRandom(new[] { 12, 24, 36, 48, 60 }))
                .RuleFor(p => p.Conditions, f => f.Lorem.Sentence())
                .RuleFor(p => p.OrganizationOrgId, _ => singleOem.OrgId)
                .RuleFor(p => p.Status, _ => "Active");
            var policies = policyFaker.Generate(recordCount);
            context.WarrantyPolicies.AddRange(policies);

            // Campaigns - keep 200
            var scTechs = employees.Where(e => e.Role == "SC_TECH").ToList();
            var campaignFaker = new Faker<Campaign>("en")
                .RuleFor(c => c.CampaignId, f => f.Database.Random.Guid())
                .RuleFor(c => c.Title, f => f.Company.Bs() + " Campaign")
                .RuleFor(c => c.Type, f => f.PickRandom(new[] { "RECALL", "SERVICE" }))
                .RuleFor(c => c.Description, f => f.Lorem.Sentence())
                .RuleFor(c => c.StartDate, f => DateOnly.FromDateTime(f.Date.Past(1)))
                .RuleFor(c => c.EndDate, f => DateOnly.FromDateTime(f.Date.Soon(90)))
                .RuleFor(c => c.Status, f => f.PickRandom(new[] { "Active", "Close" }))
                .RuleFor(c => c.CreatedBy, f => f.PickRandom(scTechs).UserId)
                .RuleFor(c => c.CreatedAt, f => f.Date.Recent(60))
                .RuleFor(c => c.PartModel, f => f.PickRandom(allModels))
                .RuleFor(c => c.ReplacementPartModel, (f, c) =>
                {
                    var candidates = allModels.Where(m => !string.Equals(m, c.PartModel, StringComparison.OrdinalIgnoreCase)).ToList();
                    return f.PickRandom(candidates);
                });
            var campaigns = campaignFaker.Generate(recordCount);
            context.Campaigns.AddRange(campaigns);

            context.SaveChanges();

            // Record basic campaign info for first few
            foreach (var c in campaigns.Take(5))
            {
                reportLines.Add($"Campaign: {c.Title} | PartModel: {c.PartModel} | Replacement: {c.ReplacementPartModel}");
            }

            // === LEVEL 2: dependent on base ===
            // Vehicles - 150 records (updated)
            var vinfastEvModels = new[] { "VF e34", "VF 3", "VF 5", "VF 6", "VF 7", "VF 8", "VF 9" };
            var vehicleFaker = new Faker<Vehicle>("en")
                .RuleFor(v => v.Vin, f => f.Random.AlphaNumeric(17).ToUpper())
                .RuleFor(v => v.Model, f => f.PickRandom(vinfastEvModels))
                .RuleFor(v => v.Year, f => f.Random.Int(2022, 2025))
                .RuleFor(v => v.CustomerId, f => f.PickRandom(customers).CustomerId);
            var vehicles = vehicleFaker.Generate(vehicleCount);
            context.Vehicles.AddRange(vehicles);

            // Vehicle parts: each vehicle has at least one model per category (updated logic)
            var vehicleParts = new List<VehiclePart>();
            var rng2 = new Random(999);
            var categories = PartModel.ModelsByCategory.Keys.ToList();
            foreach (var v in vehicles)
            {
                foreach (var cat in categories)
                {
                    var modelsInCat = PartModel.ModelsByCategory[cat];
                    var chosenModel = modelsInCat[rng2.Next(modelsInCat.Count)];
                    vehicleParts.Add(new VehiclePart
                    {
                        VehiclePartId = Guid.NewGuid(),
                        Vin = v.Vin,
                        Model = chosenModel,
                        SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                        InstalledDate = DateTime.UtcNow.AddDays(-rng2.Next(60, 600)),
                        UninstalledDate = DateTime.MinValue,
                        Status = VehiclePartStatus.Installed.GetVehiclePartStatus()
                    });
                }
            }
            // Ensure two chosen popular models appear in ~80 vehicles each
            var popularModelA = "LED Headlamps";          // from Lighting category
            var popularModelB = "Central Touchscreen";     // from Infotainment category
            int targetVehiclesPerPopularModel = 80;
            var shuffledVehicles = vehicles.OrderBy(_ => rng2.Next()).ToList();
            // Assign popularModelA
            foreach (var v in shuffledVehicles.Take(targetVehiclesPerPopularModel))
            {
                if (!vehicleParts.Any(vp => vp.Vin == v.Vin && vp.Model == popularModelA && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus()))
                {
                    vehicleParts.Add(new VehiclePart
                    {
                        VehiclePartId = Guid.NewGuid(),
                        Vin = v.Vin,
                        Model = popularModelA,
                        SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                        InstalledDate = DateTime.UtcNow.AddDays(-rng2.Next(30, 180)),
                        UninstalledDate = DateTime.MinValue,
                        Status = VehiclePartStatus.Installed.GetVehiclePartStatus()
                    });
                }
            }
            // Assign popularModelB (may overlap some vehicles, acceptable)
            foreach (var v in shuffledVehicles.Skip(targetVehiclesPerPopularModel).Take(targetVehiclesPerPopularModel))
            {
                if (!vehicleParts.Any(vp => vp.Vin == v.Vin && vp.Model == popularModelB && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus()))
                {
                    vehicleParts.Add(new VehiclePart
                    {
                        VehiclePartId = Guid.NewGuid(),
                        Vin = v.Vin,
                        Model = popularModelB,
                        SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                        InstalledDate = DateTime.UtcNow.AddDays(-rng2.Next(30, 180)),
                        UninstalledDate = DateTime.MinValue,
                        Status = VehiclePartStatus.Installed.GetVehiclePartStatus()
                    });
                }
            }
            context.VehicleParts.AddRange(vehicleParts);
            reportLines.Add($"Vehicles with {popularModelA}: {vehicleParts.Count(vp => vp.Model == popularModelA && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus())}");
            reportLines.Add($"Vehicles with {popularModelB}: {vehicleParts.Count(vp => vp.Model == popularModelB && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus())}");

            // Ensure vehicles have campaign PartModel installed for first 10 campaigns (still ensure if missing)
            foreach (var c in campaigns.Take(10))
            {
                var model = c.PartModel;
                var ensuredVins = new List<string>();
                var ensureVehicles = vehicles.OrderBy(_ => rng.Next()).Take(5).ToList();
                foreach (var v in ensureVehicles)
                {
                    var hasModel = vehicleParts.Any(vp => vp.Vin == v.Vin && vp.Model == model && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus());
                    if (!hasModel)
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
                    ensuredVins.Add(v.Vin);
                }
                reportLines.Add($"Campaign Part prepared on VINs (may include pre-existing): {string.Join(", ", ensuredVins)}");
            }
            context.SaveChanges();

            // Part Orders - keep 200
            var scOrgs = serviceCenters.ToList();
            var scEmployeeIds = employees.Where(e => e.Role == "SC_STAFF" || e.Role == "SC_TECH").Select(e => e.UserId).ToList();
            var partOrders = new List<PartOrder>();
            var partOrderItems = new List<PartOrderItem>();
            
            int ordersPerSc = recordCount / scOrgs.Count;
            foreach (var sc in scOrgs)
            {
                var scStaff = employees.Where(e => e.Role == "SC_STAFF" && e.OrgId == sc.OrgId).ToList();
                
                for (int i = 0; i < ordersPerSc; i++)
                {
                    var order = new PartOrder
                    {
                        OrderId = Guid.NewGuid(),
                        ServiceCenterId = sc.OrgId,
                        RequestDate = DateTime.UtcNow.AddDays(-rng.Next(10, 60)),
                        Status = PartOrderStatus.Waiting.GetPartOrderStatus(),
                        CreatedBy = scStaff[rng.Next(scStaff.Count)].UserId,
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

                    // 1-3 items per order
                    var orderModels = allModels.OrderBy(_ => rng.Next()).Take(rng.Next(1, 4)).ToList();
                    foreach (var model in orderModels)
                    {
                        partOrderItems.Add(new PartOrderItem
                        {
                            OrderItemId = Guid.NewGuid(),
                            OrderId = order.OrderId,
                            Model = model,
                            Quantity = rng.Next(1, 6),
                            Remarks = "Order item"
                        });
                    }
                }
            }
            context.PartOrders.AddRange(partOrders);
            context.PartOrderItems.AddRange(partOrderItems);

            context.SaveChanges();

            // === LEVEL 3: Campaign Vehicles & replacements - keep logic ===
            var campaignVehicles = new List<CampaignVehicle>();
            int vehiclesPerCampaign = Math.Max(1, recordCount / campaigns.Count);
            
            foreach (var c in campaigns)
            {
                var model = c.PartModel;
                var vehiclesWithModel = vehicles
                    .Where(v => vehicleParts.Any(vp => vp.Vin == v.Vin && vp.Model == model && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus()))
                    .OrderBy(_ => rng.Next())
                    .Take(vehiclesPerCampaign)
                    .ToList();

                foreach (var v in vehiclesWithModel)
                {
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

            var replacements = new List<CampaignVehicleReplacement>();
            foreach (var cv in campaignVehicles.Where(cv => cv.Status == CampaignVehicleStatus.Repaired.GetCampaignVehicleStatus() || cv.Status == CampaignVehicleStatus.Done.GetCampaignVehicleStatus()))
            {
                var campaign = campaigns.First(c => c.CampaignId == cv.CampaignId);
                var partModel = campaign.PartModel;
                var replacementModel = campaign.ReplacementPartModel;

                var vpInstalled = vehicleParts.Where(vp => vp.Vin == cv.Vin && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus() && vp.Model == partModel).ToList();
                if (!vpInstalled.Any()) continue;

                var toReplace = vpInstalled.OrderBy(_ => rng.Next()).Take(1).ToList();
                var newSerials = new List<string>();
                var replacedAt = cv.CompletedAt ?? DateTime.UtcNow.AddDays(-rng.Next(1, 10));

                foreach (var oldPart in toReplace)
                {
                    oldPart.Status = VehiclePartStatus.UnInstalled.GetVehiclePartStatus();
                    oldPart.UninstalledDate = replacedAt;

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

            // === Campaign Notifications - keep 200 logic ===
            var campaignNotifications = new List<CampaignNotification>();
            var now = DateTime.UtcNow;
            var activeCampaigns = campaigns.Where(c => c.Status == "Active").Take(20).ToList();
            
            foreach (var campaign in activeCampaigns)
            {
                var affectedVins = vehicleParts
                    .Where(vp => vp.Model == campaign.PartModel && 
                                 vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus())
                    .Select(vp => vp.Vin)
                    .Distinct()
                    .Take(10)
                    .ToList();

                foreach (var vin in affectedVins)
                {
                    var notificationScenario = rng.Next(0, 5);
                    CampaignNotification notification;

                    switch (notificationScenario)
                    {
                        case 0:
                            notification = new CampaignNotification
                            {
                                CampaignNotificationId = Guid.NewGuid(),
                                CampaignId = campaign.CampaignId,
                                Vin = vin,
                                EmailSentCount = 0,
                                FirstEmailSentAt = null,
                                LastEmailSentAt = null,
                                IsCompleted = false,
                                CreatedAt = now.AddDays(-rng.Next(1, 10)),
                                UpdatedAt = null
                            };
                            break;
                        case 1:
                            notification = new CampaignNotification
                            {
                                CampaignNotificationId = Guid.NewGuid(),
                                CampaignId = campaign.CampaignId,
                                Vin = vin,
                                EmailSentCount = 1,
                                FirstEmailSentAt = now.AddDays(-rng.Next(1, 5)),
                                LastEmailSentAt = now.AddDays(-rng.Next(1, 5)),
                                IsCompleted = false,
                                CreatedAt = now.AddDays(-rng.Next(5, 15)),
                                UpdatedAt = now.AddDays(-rng.Next(1, 5))
                            };
                            break;
                        case 2:
                            var firstSent = now.AddDays(-rng.Next(15, 30));
                            notification = new CampaignNotification
                            {
                                CampaignNotificationId = Guid.NewGuid(),
                                CampaignId = campaign.CampaignId,
                                Vin = vin,
                                EmailSentCount = 2,
                                FirstEmailSentAt = firstSent,
                                LastEmailSentAt = now.AddDays(-rng.Next(8, 14)),
                                IsCompleted = false,
                                CreatedAt = firstSent.AddDays(-rng.Next(1, 5)),
                                UpdatedAt = now.AddDays(-rng.Next(8, 14))
                            };
                            break;
                        case 3:
                            var completedFirst = now.AddDays(-rng.Next(20, 40));
                            notification = new CampaignNotification
                            {
                                CampaignNotificationId = Guid.NewGuid(),
                                CampaignId = campaign.CampaignId,
                                Vin = vin,
                                EmailSentCount = rng.Next(1, 3),
                                FirstEmailSentAt = completedFirst,
                                LastEmailSentAt = completedFirst.AddDays(rng.Next(1, 7)),
                                IsCompleted = true,
                                CreatedAt = completedFirst.AddDays(-rng.Next(1, 5)),
                                UpdatedAt = now.AddDays(-rng.Next(1, 5))
                            };
                            break;
                        default:
                            var maxFirst = now.AddDays(-rng.Next(30, 50));
                            notification = new CampaignNotification
                            {
                                CampaignNotificationId = Guid.NewGuid(),
                                CampaignId = campaign.CampaignId,
                                Vin = vin,
                                EmailSentCount = 3,
                                FirstEmailSentAt = maxFirst,
                                LastEmailSentAt = now.AddDays(-rng.Next(1, 7)),
                                IsCompleted = false,
                                CreatedAt = maxFirst.AddDays(-rng.Next(1, 5)),
                                UpdatedAt = now.AddDays(-rng.Next(1, 7))
                            };
                            break;
                    }

                    campaignNotifications.Add(notification);
                    if (campaignNotifications.Count >= recordCount)
                        break;
                }
                if (campaignNotifications.Count >= recordCount)
                    break;
            }

            context.CampaignNotifications.AddRange(campaignNotifications);
            context.SaveChanges();

            reportLines.Add($"Campaign Notifications created: {campaignNotifications.Count}");
            reportLines.Add($"  - Never sent: {campaignNotifications.Count(n => n.EmailSentCount == 0)}");
            reportLines.Add($"  - Sent 1 time: {campaignNotifications.Count(n => n.EmailSentCount == 1)}");
            reportLines.Add($"  - Sent 2 times: {campaignNotifications.Count(n => n.EmailSentCount == 2)}");
            reportLines.Add($"  - Sent 3 times (max): {campaignNotifications.Count(n => n.EmailSentCount == 3)}");
            reportLines.Add($"  - Completed: {campaignNotifications.Count(n => n.IsCompleted)}");

            // === LEVEL 4: Warranty Claims + related - keep at 200 ===
            var vehicleWarrantyPolicies = new List<VehicleWarrantyPolicy>();
            for (int i = 0; i < recordCount; i++)
            {
                var vehicle = vehicles[i % vehicles.Count];
                var policy = policies[i % policies.Count];
                vehicleWarrantyPolicies.Add(new VehicleWarrantyPolicy
                {
                    VehicleWarrantyId = Guid.NewGuid(),
                    Vin = vehicle.Vin,
                    PolicyId = policy.PolicyId,
                    StartDate = DateTime.UtcNow.AddMonths(-rng.Next(1, 12)),
                    EndDate = DateTime.UtcNow.AddMonths(rng.Next(12, 48)),
                    Status = rng.NextDouble() < 0.8 ? "Active" : "Expired"
                });
            }
            context.VehicleWarrantyPolicies.AddRange(vehicleWarrantyPolicies);
            context.SaveChanges();

            var claims = new List<WarrantyClaim>();
            var evmStaffs = employees.Where(e => e.Role == "EVM_STAFF").ToList();

            string PickInstalledModel(string vin)
            {
                var vp = vehicleParts.FirstOrDefault(vp => vp.Vin == vin && vp.Status == VehiclePartStatus.Installed.GetVehiclePartStatus());
                return vp?.Model ?? allModels[rng.Next(allModels.Count)];
            }

            var claimStatuses = new[]
            {
                WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.Approved.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.Repaired.GetWarrantyClaimStatus()
            };

            int claimsPerSc = recordCount / scOrgs.Count;
            foreach (var sc in scOrgs)
            {
                var scStaff = employees.Where(e => e.Role == "SC_STAFF" && e.OrgId == sc.OrgId).ToList();
                var scTech = employees.Where(e => e.Role == "SC_TECH" && e.OrgId == sc.OrgId).ToList();
                
                for (int i = 0; i < claimsPerSc; i++)
                {
                    var veh = vehicles[rng.Next(vehicles.Count)];
                    var status = claimStatuses[rng.Next(claimStatuses.Length)];
                    var evmStaff = evmStaffs[rng.Next(evmStaffs.Count)];
                    
                    var c = new WarrantyClaim
                    {
                        ClaimId = Guid.NewGuid(),
                        Vin = veh.Vin,
                        ServiceCenterId = sc.OrgId,
                        CreatedBy = scStaff[rng.Next(scStaff.Count)].UserId,
                        CreatedDate = DateTime.UtcNow.AddDays(-rng.Next(2, 60)),
                        Status = status,
                        Description = "Customer reports issue",
                        failureDesc = "Component failure"
                    };

                    // Set additional fields based on status
                    if (status != WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus())
                    {
                        c.VehicleWarrantyId = vehicleWarrantyPolicies[rng.Next(vehicleWarrantyPolicies.Count)].VehicleWarrantyId;
                    }

                    if (status == WarrantyClaimStatus.Approved.GetWarrantyClaimStatus() || 
                        status == WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus() ||
                        status == WarrantyClaimStatus.Repaired.GetWarrantyClaimStatus())
                    {
                        c.ConfirmBy = evmStaff.UserId;
                        c.ConfirmDate = DateTime.UtcNow.AddDays(-rng.Next(1, 10));
                    }

                    claims.Add(c);

                    // Add claim parts
                    var claimPartStatus = status == WarrantyClaimStatus.Approved.GetWarrantyClaimStatus() || 
                                          status == WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus() ||
                                          status == WarrantyClaimStatus.Repaired.GetWarrantyClaimStatus()
                        ? ClaimPartStatus.Enough.GetClaimPartStatus()
                        : ClaimPartStatus.Pending.GetClaimPartStatus();

                    context.ClaimParts.Add(new ClaimPart
                    {
                        ClaimPartId = Guid.NewGuid(),
                        ClaimId = c.ClaimId,
                        Model = PickInstalledModel(veh.Vin),
                        SerialNumberOld = vehicleParts.FirstOrDefault(vp => vp.Vin == veh.Vin)?.SerialNumber ?? "UNKNOWN",
                        SerialNumberNew = claimPartStatus == ClaimPartStatus.Enough.GetClaimPartStatus() 
                            ? $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}" 
                            : null,
                        Action = ClaimPartAction.Replace.GetClaimPartAction(),
                        Status = claimPartStatus,
                        Cost = rng.Next(100, 1000)
                    });

                    // Create work orders for under inspection and under repair
                    if (status == WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus())
                    {
                        context.WorkOrders.Add(new WorkOrder
                        {
                            WorkOrderId = Guid.NewGuid(),
                            AssignedTo = scTech[rng.Next(scTech.Count)].UserId,
                            Type = WorkOrderType.Inspection.GetWorkOrderType(),
                            Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                            TargetId = c.ClaimId,
                            Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                            StartDate = DateTime.UtcNow.AddDays(-rng.Next(1, 5))
                        });
                    }
                    else if (status == WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus())
                    {
                        context.WorkOrders.Add(new WorkOrder
                        {
                            WorkOrderId = Guid.NewGuid(),
                            AssignedTo = scTech[rng.Next(scTech.Count)].UserId,
                            Type = WorkOrderType.Repair.GetWorkOrderType(),
                            Target = WorkOrderTarget.Warranty.GetWorkOrderTarget(),
                            TargetId = c.ClaimId,
                            Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                            StartDate = DateTime.UtcNow.AddDays(-rng.Next(1, 5))
                        });
                    }
                }
            }

            context.WarrantyClaims.AddRange(claims);
            context.SaveChanges();

            var attachments = new Faker<ClaimAttachment>("en")
                .RuleFor(a => a.AttachmentId, f => f.Database.Random.Guid().ToString())
                .RuleFor(a => a.ClaimId, f => f.PickRandom(claims).ClaimId)
                .RuleFor(a => a.URL, f => f.Image.PicsumUrl())
                .RuleFor(a => a.UploadedBy, f => f.PickRandom(scEmployeeIds))
                .Generate(recordCount);
            context.ClaimAttachments.AddRange(attachments);

            var backClaims = new Faker<BackWarrantyClaim>("en")
                .RuleFor(b => b.WarrantyClaimId, f => f.PickRandom(claims).ClaimId)
                .RuleFor(b => b.CreatedDate, f => f.Date.Recent(30))
                .RuleFor(b => b.Description, f => f.Lorem.Sentence(6))
                .RuleFor(b => b.CreatedByEmployeeId, f => f.PickRandom(scEmployeeIds))
                .Generate(recordCount);
            context.BackWarrantyClaims.AddRange(backClaims);

            var appointments = new Faker<Appointment>("en")
                .RuleFor(a => a.AppointmentId, f => f.Database.Random.Guid())
                .RuleFor(a => a.AppointmentType, f => f.PickRandom(new[] { "Warranty", "Campaign" }))
                .RuleFor(a => a.Vin, f => f.PickRandom(vehicles).Vin)
                .RuleFor(a => a.ServiceCenterId, f => f.PickRandom(scOrgs).OrgId)
                .RuleFor(a => a.AppointmentDate, f => DateOnly.FromDateTime(f.Date.Soon(30)))
                .RuleFor(a => a.Slot, f => f.PickRandom(new[] { "Slot1", "Slot2", "Slot3", "Slot4", "Slot5", "Slot6", "Slot7", "Slot8" }))
                .RuleFor(a => a.Status, f => f.PickRandom(new[] { "Scheduled", "Checked_in", "Canceled", "Done", "No_show", "Pending" }))
                .RuleFor(a => a.CreatedAt, f => f.Date.Recent(60))
                .RuleFor(a => a.Note, f => f.Random.Bool(0.4f) ? f.Lorem.Sentence() : null)
                .Generate(recordCount);
            context.Appointments.AddRange(appointments);

            context.SaveChanges();

            // Emit report lines to console and file to help testers
            try
            {
                Console.WriteLine("==== Seed Report (for quick testing) ====");
                Console.WriteLine($"Total Organizations: {organizations.Count} (1 OEM + 3 Service Centers)");
                Console.WriteLine($"Total Employees: {employees.Count} (1 Admin, 2 EVM Staff, 6 SC Staff, 15 SC Tech)");
                Console.WriteLine($"Total Customers: {customers.Count}");
                Console.WriteLine($"Total Vehicles: {vehicles.Count}");
                Console.WriteLine($"Total Parts: {parts.Count}");
                Console.WriteLine($"Total Policies: {policies.Count}");
                Console.WriteLine($"Total Campaigns: {campaigns.Count}");
                Console.WriteLine($"Total Campaign Vehicles: {campaignVehicles.Count}");
                Console.WriteLine($"Total Campaign Notifications: {campaignNotifications.Count}");
                Console.WriteLine($"Total Part Orders: {partOrders.Count}");
                Console.WriteLine($"Total Warranty Claims: {claims.Count}");
                Console.WriteLine($"Total Appointments: {appointments.Count}");
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