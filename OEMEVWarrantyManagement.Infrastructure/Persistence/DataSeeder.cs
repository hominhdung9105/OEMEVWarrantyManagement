using Bogus;
using OEMEVWarrantyManagement.Domain.Entities;
using OEMEVWarrantyManagement.Share.Enums;
using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace OEMEVWarrantyManagement.Infrastructure.Persistence
{
    public static class DataSeeder
    {
        public static void SeedDatabase(AppDbContext context)
        {
            const int recordCount = 200;
            const int customerCount = 50;
            const int vehicleCount = 150;
            const int campaignCount = 3;
            const int claimCount = 40;
            Randomizer.Seed = new Random(12345);

            if (context.Organizations.Any()) return;

            var reportLines = new List<string>();

            // === Organizations ===
            var oems = new List<Organization>
            {
                new Organization
                {
                    OrgId = Guid.NewGuid(),
                    Name = "Vietnam",
                    Region = "Asia",
                    Type = "OEM",
                    ContactInfo = "123-456-7890"
                }
            };
            var serviceCenters = new List<Organization>
            {
                new Organization
                {
                    OrgId = Guid.NewGuid(),
                    Name = "Japan",
                    Region = "Asia",
                    Type = "ServiceCenter",
                    ContactInfo = "123-456-7891"
                },
                new Organization
                {
                    OrgId = Guid.NewGuid(),
                    Name = "Pizza",
                    Region = "Europe",
                    Type = "ServiceCenter",
                    ContactInfo = "123-456-7892"
                },
                new Organization
                {
                    OrgId = Guid.NewGuid(),
                    Name = "Black",
                    Region = "Africa",
                    Type = "ServiceCenter",
                    ContactInfo = "123-456-7893"
                }
            };
            var organizations = new List<Organization>();
            organizations.AddRange(oems);
            organizations.AddRange(serviceCenters);
            context.Organizations.AddRange(organizations);
            context.SaveChanges();

            var singleOem = oems.First();

            // === Employees ===
            var passwordHasher = new PasswordHasher<Employee>();
            var employees = new List<Employee>();
            // ADMIN for Vietnam
            employees.Add(new Employee
            {
                UserId = Guid.NewGuid(),
                Email = "VietnamAD1@gmail.com",
                Name = "Admin User",
                PasswordHash = passwordHasher.HashPassword(new Employee(), "P@ssW0rd"),
                Role = "ADMIN",
                OrgId = singleOem.OrgId
            });
            // EVM_STAFF for Vietnam
            employees.Add(new Employee
            {
                UserId = Guid.NewGuid(),
                Email = "VietnamEVM1@gmail.com",
                Name = "EVM Staff 1",
                PasswordHash = passwordHasher.HashPassword(new Employee(), "P@ssW0rd"),
                Role = "EVM_STAFF",
                OrgId = singleOem.OrgId
            });
            employees.Add(new Employee
            {
                UserId = Guid.NewGuid(),
                Email = "VietnamEVM2@gmail.com",
                Name = "EVM Staff 2",
                PasswordHash = passwordHasher.HashPassword(new Employee(), "P@ssW0rd"),
                Role = "EVM_STAFF",
                OrgId = singleOem.OrgId
            });
            // For each SC
            foreach (var sc in serviceCenters)
            {
                // SC_STAFF: 2
                employees.Add(new Employee
                {
                    UserId = Guid.NewGuid(),
                    Email = $"{sc.Name}SC1@gmail.com",
                    Name = $"{sc.Name} SC Staff 1",
                    PasswordHash = passwordHasher.HashPassword(new Employee(), "P@ssW0rd"),
                    Role = "SC_STAFF",
                    OrgId = sc.OrgId
                });
                employees.Add(new Employee
                {
                    UserId = Guid.NewGuid(),
                    Email = $"{sc.Name}SC2@gmail.com",
                    Name = $"{sc.Name} SC Staff 2",
                    PasswordHash = passwordHasher.HashPassword(new Employee(), "P@ssW0rd"),
                    Role = "SC_STAFF",
                    OrgId = sc.OrgId
                });
                // SC_TECH: 5
                for (int i = 1; i <= 5; i++)
                {
                    employees.Add(new Employee
                    {
                        UserId = Guid.NewGuid(),
                        Email = $"{sc.Name}TECH{i}@gmail.com",
                        Name = $"{sc.Name} SC Tech {i}",
                        PasswordHash = passwordHasher.HashPassword(new Employee(), "P@ssW0rd"),
                        Role = "SC_TECH",
                        OrgId = sc.OrgId
                    });
                }
            }
            context.Employees.AddRange(employees);

            // === Customers ===
            var customerFaker = new Faker<Customer>("en")
                .RuleFor(c => c.CustomerId, f => f.Database.Random.Guid())
                .RuleFor(c => c.Name, f => f.Name.FullName())
                .RuleFor(c => c.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(c => c.Email, (f, u) => f.Internet.Email(u.Name))
                .RuleFor(c => c.Address, f => f.Address.FullAddress())
                .RuleFor(c => c.OrganizationOrgId, _ => singleOem.OrgId);
            var customers = customerFaker.Generate(customerCount);
            context.Customers.AddRange(customers);

            // === Parts ===
            var allModels = PartModel.ModelsByCategory.SelectMany(kvp => kvp.Value).Distinct().ToList();
            var rng = new Random(777);
            var parts = new List<Part>();
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

            // === Policies ===
            var policyFaker = new Faker<WarrantyPolicy>("en")
                .RuleFor(p => p.PolicyId, f => f.Database.Random.Guid())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName() + " Warranty")
                .RuleFor(p => p.CoveragePeriodMonths, f => f.PickRandom(new[] { 12, 24, 36, 48, 60 }))
                .RuleFor(p => p.Conditions, f => f.Lorem.Sentence())
                .RuleFor(p => p.OrganizationOrgId, _ => singleOem.OrgId)
                .RuleFor(p => p.Status, _ => "Active");
            var policies = policyFaker.Generate(recordCount);
            context.WarrantyPolicies.AddRange(policies);

            // === Campaigns ===
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
            var campaigns = campaignFaker.Generate(campaignCount);
            context.Campaigns.AddRange(campaigns);
            context.SaveChanges();

            foreach (var c in campaigns.Take(5))
                reportLines.Add($"Campaign: {c.Title} | PartModel: {c.PartModel} | Replacement: {c.ReplacementPartModel}");

            Guid PickServiceCenterId(Random r) => serviceCenters[r.Next(serviceCenters.Count)].OrgId;

            // === Vehicles ===
            var vinfastEvModels = new[] { "VF e34", "VF 3", "VF 5", "VF 6", "VF 7", "VF 8", "VF 9" };
            var vehicleFaker = new Faker<Vehicle>("en")
                .RuleFor(v => v.Vin, f => f.Random.AlphaNumeric(17).ToUpper())
                .RuleFor(v => v.Model, f => f.PickRandom(vinfastEvModels))
                .RuleFor(v => v.Year, f => f.Random.Int(2022, 2025))
                .RuleFor(v => v.CustomerId, f => f.PickRandom(customers).CustomerId);
            var vehicles = vehicleFaker.Generate(vehicleCount);
            context.Vehicles.AddRange(vehicles);

            // === VehiclePartHistory Seeding (Adjusted Requirements) ===
            var vehiclePartHistories = new List<VehiclePartHistory>();
            var rng2 = new Random(999);

            // add for evm
            var evmParts = parts.Where(p => p.OrgId == oems[0].OrgId).ToList();
            foreach (var part in evmParts)
            {
                if (part.StockQuantity <= 0) continue;
                for (int i = 0; i < part.StockQuantity; i++)
                {
                    var productionDate = DateTime.UtcNow.AddDays(-rng2.Next(30, 180));
                    var warrantyMonths = rng2.Next(12, 49);
                    vehiclePartHistories.Add(new VehiclePartHistory
                    {
                        VehiclePartHistoryId = Guid.NewGuid(),
                        Vin = null, // inventory item
                        Model = part.Model,
                        SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                        InstalledAt = DateTime.MinValue, // not installed
                        UninstalledAt = DateTime.MinValue,
                        ProductionDate = productionDate,
                        WarrantyPeriodMonths = warrantyMonths,
                        WarrantyEndDate = productionDate.AddMonths(warrantyMonths),
                        ServiceCenterId = oems[0].OrgId,
                        Condition = VehiclePartCondition.New.GetCondition(),
                        Status = VehiclePartCurrentStatus.InStock.GetCurrentStatus(),
                        Note = "Seed new inventory"
                    });
                }
            }

            // 1. Inventory: For each SC part, create New condition InStock entries equal to StockQuantity (VIN null)
            foreach (var sc in serviceCenters)
            {
                var scParts = parts.Where(p => p.OrgId == sc.OrgId).ToList();
                foreach (var part in scParts)
                {
                    if (part.StockQuantity <= 0) continue;
                    for (int i = 0; i < part.StockQuantity; i++)
                    {
                        var productionDate = DateTime.UtcNow.AddDays(-rng2.Next(30, 180));
                        var warrantyMonths = rng2.Next(12, 49);
                        vehiclePartHistories.Add(new VehiclePartHistory
                        {
                            VehiclePartHistoryId = Guid.NewGuid(),
                            Vin = null, // inventory item
                            Model = part.Model,
                            SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                            InstalledAt = DateTime.MinValue, // not installed
                            UninstalledAt = DateTime.MinValue,
                            ProductionDate = productionDate,
                            WarrantyPeriodMonths = warrantyMonths,
                            WarrantyEndDate = productionDate.AddMonths(warrantyMonths),
                            ServiceCenterId = sc.OrgId,
                            Condition = VehiclePartCondition.New.GetCondition(),
                            Status = VehiclePartCurrentStatus.InStock.GetCurrentStatus(),
                            Note = "Seed new inventory"
                        });
                    }
                }
            }

            // 2. Each SC: some Defective InStock (inventory defective)
            foreach (var sc in serviceCenters)
            {
                var scModels = parts.Where(p => p.OrgId == sc.OrgId && p.StockQuantity > 0).Select(p => p.Model).OrderBy(_ => rng2.Next()).Take(5).ToList();
                foreach (var model in scModels)
                {
                    vehiclePartHistories.Add(new VehiclePartHistory
                    {
                        VehiclePartHistoryId = Guid.NewGuid(),
                        Vin = null,
                        Model = model,
                        SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                        InstalledAt = DateTime.MinValue,
                        UninstalledAt = DateTime.MinValue,
                        ProductionDate = DateTime.UtcNow.AddDays(-rng2.Next(60, 360)),
                        WarrantyPeriodMonths = 0,
                        WarrantyEndDate = DateTime.UtcNow,
                        ServiceCenterId = sc.OrgId,
                        Condition = VehiclePartCondition.Defective.GetCondition(),
                        Status = VehiclePartCurrentStatus.InStock.GetCurrentStatus(),
                        Note = "Defective inventory"
                    });
                }
            }

            // 3. Each SC: some Refurbished (half InStock, half OnVehicle)
            foreach (var sc in serviceCenters)
            {
                var refurbModels = parts.Where(p => p.OrgId == sc.OrgId).Select(p => p.Model).OrderBy(_ => rng2.Next()).Take(6).ToList();
                for (int i = 0; i < refurbModels.Count; i++)
                {
                    bool onVehicle = i % 2 == 1; // half on vehicle
                    string? vin = onVehicle ? vehicles[rng2.Next(vehicles.Count)].Vin : null;
                    DateTime installedAt = onVehicle ? DateTime.UtcNow.AddDays(-rng2.Next(5, 120)) : DateTime.MinValue;
                    int warrantyMonths = rng2.Next(6, 25);
                    vehiclePartHistories.Add(new VehiclePartHistory
                    {
                        VehiclePartHistoryId = Guid.NewGuid(),
                        Vin = vin,
                        Model = refurbModels[i],
                        SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                        InstalledAt = installedAt,
                        UninstalledAt = DateTime.MinValue,
                        ProductionDate = installedAt == DateTime.MinValue ? DateTime.UtcNow.AddDays(-rng2.Next(60, 300)) : installedAt.AddDays(-rng2.Next(30, 180)),
                        WarrantyPeriodMonths = warrantyMonths,
                        WarrantyEndDate = (installedAt == DateTime.MinValue ? DateTime.UtcNow : installedAt).AddMonths(warrantyMonths),
                        ServiceCenterId = sc.OrgId,
                        Condition = VehiclePartCondition.Refurbished.GetCondition(),
                        Status = onVehicle ? VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus() : VehiclePartCurrentStatus.InStock.GetCurrentStatus(),
                        Note = onVehicle ? "Refurbished installed" : "Refurbished inventory"
                    });
                }
            }

            // 4. Each SC: some Used InStock
            foreach (var sc in serviceCenters)
            {
                var usedModels = parts.Where(p => p.OrgId == sc.OrgId).Select(p => p.Model).OrderBy(_ => rng2.Next()).Take(5).ToList();
                foreach (var model in usedModels)
                {
                    int warrantyMonths = rng2.Next(3, 13);
                    var productionDate = DateTime.UtcNow.AddDays(-rng2.Next(180, 720));
                    vehiclePartHistories.Add(new VehiclePartHistory
                    {
                        VehiclePartHistoryId = Guid.NewGuid(),
                        Vin = null,
                        Model = model,
                        SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                        InstalledAt = DateTime.MinValue,
                        UninstalledAt = DateTime.MinValue,
                        ProductionDate = productionDate,
                        WarrantyPeriodMonths = warrantyMonths,
                        WarrantyEndDate = productionDate.AddMonths(warrantyMonths),
                        ServiceCenterId = sc.OrgId,
                        Condition = VehiclePartCondition.Used.GetCondition(),
                        Status = VehiclePartCurrentStatus.InStock.GetCurrentStatus(),
                        Note = "Used inventory"
                    });
                }
            }

            // 5. Around 500 Used OnVehicle entries (~3-4 per vehicle)
            int targetUsedOnVehicle = 500;
            int perVehicle = (int)Math.Ceiling(targetUsedOnVehicle / (double)vehicles.Count); // ~4
            var modelsPool = allModels.ToList();
            foreach (var v in vehicles)
            {
                for (int i = 0; i < perVehicle; i++)
                {
                    if (vehiclePartHistories.Count(h => h.Status == VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus() && h.Condition == VehiclePartCondition.Used.GetCondition()) >= targetUsedOnVehicle)
                        break;

                    var model = modelsPool[rng2.Next(modelsPool.Count)];
                    var installedAt = DateTime.UtcNow.AddDays(-rng2.Next(30, 600));
                    int warrantyMonths = rng2.Next(3, 25);
                    vehiclePartHistories.Add(new VehiclePartHistory
                    {
                        VehiclePartHistoryId = Guid.NewGuid(),
                        Vin = v.Vin,
                        Model = model,
                        SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                        InstalledAt = installedAt,
                        UninstalledAt = DateTime.MinValue,
                        ProductionDate = installedAt.AddDays(-rng2.Next(60, 360)),
                        WarrantyPeriodMonths = warrantyMonths,
                        WarrantyEndDate = installedAt.AddMonths(warrantyMonths),
                        ServiceCenterId = PickServiceCenterId(rng2),
                        Condition = VehiclePartCondition.Used.GetCondition(),
                        Status = VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus(),
                        Note = "Used installed part"
                    });
                }
            }

            // 6. Ensure first 10 campaigns have their PartModel installed (if missing) using Refurbished or Used randomly
            foreach (var c in campaigns.Take(10))
            {
                var model = c.PartModel;
                var ensureVehicles = vehicles.OrderBy(_ => rng.Next()).Take(5).ToList();
                foreach (var v in ensureVehicles)
                {
                    var hasModel = vehiclePartHistories.Any(vp => vp.Vin == v.Vin && vp.Model == model && vp.Status == VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus());
                    if (!hasModel)
                    {
                        bool refurb = rng.NextDouble() < 0.5;
                        var installedAt = DateTime.UtcNow.AddDays(-rng.Next(30, 180));
                        int warrantyMonths = rng.Next(6, 25);
                        vehiclePartHistories.Add(new VehiclePartHistory
                        {
                            VehiclePartHistoryId = Guid.NewGuid(),
                            Vin = v.Vin,
                            Model = model!,
                            SerialNumber = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}",
                            InstalledAt = installedAt,
                            UninstalledAt = DateTime.MinValue,
                            ProductionDate = installedAt.AddDays(-rng.Next(30, 180)),
                            WarrantyPeriodMonths = warrantyMonths,
                            WarrantyEndDate = installedAt.AddMonths(warrantyMonths),
                            ServiceCenterId = PickServiceCenterId(rng),
                            Condition = refurb ? VehiclePartCondition.Refurbished.GetCondition() : VehiclePartCondition.Used.GetCondition(),
                            Status = VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus(),
                            Note = "Campaign ensured part"
                        });
                    }
                }
            }

            context.VehiclePartHistories.AddRange(vehiclePartHistories);
            context.SaveChanges();

            reportLines.Add($"Total VehiclePartHistory inventory (New InStock): {vehiclePartHistories.Count(vp => vp.Condition == VehiclePartCondition.New.GetCondition() && vp.Status == VehiclePartCurrentStatus.InStock.GetCurrentStatus())}");
            reportLines.Add($"Total VehiclePartHistory Used OnVehicle: {vehiclePartHistories.Count(vp => vp.Condition == VehiclePartCondition.Used.GetCondition() && vp.Status == VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus())}");

            // === Part Orders (unchanged) ===
            var scOrgs = serviceCenters.ToList();
            var scEmployeeUserIds = employees.Where(e => e.Role == "SC_STAFF" || e.Role == "SC_TECH").Select(e => e.UserId).ToList();
            var partOrders = new List<PartOrder>();
            var partOrderItems = new List<PartOrderItem>();
            int ordersPerSc = 10;
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

            // === Campaign Vehicles & replacements (adjust replacement condition to Refurbished) ===
            var campaignVehicles = new List<CampaignVehicle>();
            int vehiclesPerCampaign = Math.Max(1, recordCount / campaigns.Count);
            foreach (var c in campaigns)
            {
                var model = c.PartModel;
                var vehiclesWithModel = vehicles
                    .Where(v => vehiclePartHistories.Any(vp => vp.Vin == v.Vin && vp.Model == model && vp.Status == VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus()))
                    .OrderBy(_ => rng.Next()).Take(vehiclesPerCampaign).ToList();
                foreach (var v in vehiclesWithModel)
                {
                    var statusPick = rng.Next(0, 3);
                    var cv = new CampaignVehicle
                    {
                        CampaignVehicleId = Guid.NewGuid(),
                        CampaignId = c.CampaignId,
                        Vin = v.Vin,
                        CreatedAt = DateTime.UtcNow.AddDays(-rng.Next(5, 50)),
                        Status = statusPick == 0 ? CampaignVehicleStatus.WaitingForUnassignedRepair.GetCampaignVehicleStatus() :
                                 statusPick == 1 ? CampaignVehicleStatus.UnderRepair.GetCampaignVehicleStatus() :
                                                  CampaignVehicleStatus.Repaired.GetCampaignVehicleStatus(),
                    };
                    if (cv.Status == CampaignVehicleStatus.Repaired.GetCampaignVehicleStatus())
                        cv.CompletedAt = DateTime.UtcNow.AddDays(-rng.Next(1, 5));
                    campaignVehicles.Add(cv);
                }
            }
            context.CampaignVehicles.AddRange(campaignVehicles);
            context.SaveChanges();

            var campaignUnderRepair = campaignVehicles.Where(x => x.Status == CampaignVehicleStatus.UnderRepair.GetCampaignVehicleStatus()).ToList();
            foreach (var cv in campaignUnderRepair)
            {
                var anySc = scTechs.OrderBy(_ => rng.Next()).First();
                context.WorkOrders.Add(new WorkOrder
                {
                    WorkOrderId = Guid.NewGuid(),
                    AssignedTo = anySc.UserId,
                    Type = WorkOrderType.Repair.GetWorkOrderType(),
                    Target = WorkOrderTarget.Campaign.GetWorkOrderTarget(),
                    TargetId = cv.CampaignVehicleId,
                    Status = WorkOrderStatus.InProgress.GetWorkOrderStatus(),
                    StartDate = DateTime.UtcNow.AddDays(-rng.Next(1, 7))
                });
            }

            var replacements = new List<CampaignVehicleReplacement>();
            foreach (var cv in campaignVehicles.Where(cv => cv.Status == CampaignVehicleStatus.Repaired.GetCampaignVehicleStatus() || cv.Status == CampaignVehicleStatus.Done.GetCampaignVehicleStatus()))
            {
                var campaign = campaigns.First(c => c.CampaignId == cv.CampaignId);
                var partModel = campaign.PartModel;
                var replacementModel = campaign.ReplacementPartModel;
                var vpInstalled = vehiclePartHistories.Where(vp => vp.Vin == cv.Vin && vp.Status == VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus() && vp.Model == partModel).ToList();
                if (!vpInstalled.Any()) continue;
                var toReplace = vpInstalled.OrderBy(_ => rng.Next()).Take(1).ToList();
                var newSerials = new List<string>();
                var replacedAt = cv.CompletedAt ?? DateTime.UtcNow.AddDays(-rng.Next(1, 10));
                foreach (var oldPart in toReplace)
                {
                    oldPart.Status = VehiclePartCurrentStatus.Returned.GetCurrentStatus();
                    oldPart.UninstalledAt = replacedAt;
                    oldPart.Condition = VehiclePartCondition.Defective.GetCondition();
                    oldPart.Note = "Replaced by campaign";
                    var newSerial = $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}";
                    int warrantyMonths = rng.Next(6, 25);
                    var newPartHistory = new VehiclePartHistory
                    {
                        VehiclePartHistoryId = Guid.NewGuid(),
                        Vin = cv.Vin,
                        Model = replacementModel!,
                        SerialNumber = newSerial,
                        InstalledAt = replacedAt,
                        UninstalledAt = DateTime.MinValue,
                        ProductionDate = replacedAt.AddDays(-rng.Next(10, 60)),
                        WarrantyPeriodMonths = warrantyMonths,
                        WarrantyEndDate = replacedAt.AddMonths(warrantyMonths),
                        ServiceCenterId = PickServiceCenterId(rng),
                        Condition = VehiclePartCondition.Refurbished.GetCondition(), // adjusted
                        Status = VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus(),
                        Note = "Replacement refurbished part installed"
                    };
                    context.VehiclePartHistories.Add(newPartHistory);
                    vehiclePartHistories.Add(newPartHistory);
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
            if (replacements.Count > 0) context.CampaignVehicleReplacements.AddRange(replacements);
            context.SaveChanges();

            // === Campaign Notifications === (unchanged logic except counts)
            var campaignNotifications = new List<CampaignNotification>();
            var now = DateTime.UtcNow;
            var activeCampaigns = campaigns.Where(c => c.Status == "Active").Take(20).ToList();
            foreach (var campaign in activeCampaigns)
            {
                var affectedVins = vehiclePartHistories.Where(vp => vp.Model == campaign.PartModel && vp.Status == VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus()).Select(vp => vp.Vin).Where(v => v != null).Distinct().Take(10).ToList();
                foreach (var vin in affectedVins)
                {
                    var notificationScenario = rng.Next(0, 5);
                    CampaignNotification notification = notificationScenario switch
                    {
                        0 => new CampaignNotification { CampaignNotificationId = Guid.NewGuid(), CampaignId = campaign.CampaignId, Vin = vin!, EmailSentCount = 0, FirstEmailSentAt = null, LastEmailSentAt = null, IsCompleted = false, CreatedAt = now.AddDays(-rng.Next(1, 10)), UpdatedAt = null },
                        1 => new CampaignNotification { CampaignNotificationId = Guid.NewGuid(), CampaignId = campaign.CampaignId, Vin = vin!, EmailSentCount = 1, FirstEmailSentAt = now.AddDays(-rng.Next(1, 5)), LastEmailSentAt = now.AddDays(-rng.Next(1, 5)), IsCompleted = false, CreatedAt = now.AddDays(-rng.Next(5, 15)), UpdatedAt = now.AddDays(-rng.Next(1, 5)) },
                        2 => new CampaignNotification { CampaignNotificationId = Guid.NewGuid(), CampaignId = campaign.CampaignId, Vin = vin!, EmailSentCount = 2, FirstEmailSentAt = now.AddDays(-rng.Next(15, 30)), LastEmailSentAt = now.AddDays(-rng.Next(8, 14)), IsCompleted = false, CreatedAt = now.AddDays(-rng.Next(20, 35)), UpdatedAt = now.AddDays(-rng.Next(8, 14)) },
                        3 => new CampaignNotification { CampaignNotificationId = Guid.NewGuid(), CampaignId = campaign.CampaignId, Vin = vin!, EmailSentCount = rng.Next(1, 3), FirstEmailSentAt = now.AddDays(-rng.Next(20, 40)), LastEmailSentAt = now.AddDays(-rng.Next(1, 7)), IsCompleted = true, CreatedAt = now.AddDays(-rng.Next(25, 45)), UpdatedAt = now.AddDays(-rng.Next(1, 5)) },
                        _ => new CampaignNotification { CampaignNotificationId = Guid.NewGuid(), CampaignId = campaign.CampaignId, Vin = vin!, EmailSentCount = 3, FirstEmailSentAt = now.AddDays(-rng.Next(30, 50)), LastEmailSentAt = now.AddDays(-rng.Next(1, 7)), IsCompleted = false, CreatedAt = now.AddDays(-rng.Next(35, 55)), UpdatedAt = now.AddDays(-rng.Next(1, 7)) }
                    };
                    campaignNotifications.Add(notification);
                    if (campaignNotifications.Count >= recordCount) break;
                }
                if (campaignNotifications.Count >= recordCount) break;
            }
            context.CampaignNotifications.AddRange(campaignNotifications);
            context.SaveChanges();

            reportLines.Add($"Campaign Notifications created: {campaignNotifications.Count}");

            // === Warranty Policies on Vehicles ===
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

            // === Warranty Claims === (unchanged)
            var claims = new List<WarrantyClaim>();
            var evmStaffs = employees.Where(e => e.Role == "EVM_STAFF").ToList();
            string PickInstalledModel(string vin) => vehiclePartHistories.FirstOrDefault(vp => vp.Vin == vin && vp.Status == VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus())?.Model ?? allModels[rng.Next(allModels.Count)];
            var claimStatuses = new[]
            {
                WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.UnderInspection.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.Approved.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus(),
                WarrantyClaimStatus.Repaired.GetWarrantyClaimStatus()
            };
            int basePerSc = Math.Max(0, claimCount / scOrgs.Count);
            int remainder = claimCount % scOrgs.Count;
            for (int scIdx = 0; scIdx < scOrgs.Count; scIdx++)
            {
                var sc = scOrgs[scIdx];
                var scStaff = employees.Where(e => e.Role == "SC_STAFF" && e.OrgId == sc.OrgId).ToList();
                var scTech = employees.Where(e => e.Role == "SC_TECH" && e.OrgId == sc.OrgId).ToList();
                int claimsForThisSc = 10;
                for (int i = 0; i < claimsForThisSc; i++)
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
                    if (status != WarrantyClaimStatus.WaitingForUnassigned.GetWarrantyClaimStatus())
                        c.VehicleWarrantyId = vehicleWarrantyPolicies[rng.Next(vehicleWarrantyPolicies.Count)].VehicleWarrantyId;
                    if (status == WarrantyClaimStatus.Approved.GetWarrantyClaimStatus() || status == WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus() || status == WarrantyClaimStatus.Repaired.GetWarrantyClaimStatus())
                    {
                        c.ConfirmBy = evmStaff.UserId;
                        c.ConfirmDate = DateTime.UtcNow.AddDays(-rng.Next(1, 10));
                    }
                    claims.Add(c);
                    var claimPartStatus = (status == WarrantyClaimStatus.Approved.GetWarrantyClaimStatus() || status == WarrantyClaimStatus.UnderRepair.GetWarrantyClaimStatus() || status == WarrantyClaimStatus.Repaired.GetWarrantyClaimStatus())
                        ? ClaimPartStatus.Enough.GetClaimPartStatus() : ClaimPartStatus.Pending.GetClaimPartStatus();
                    context.ClaimParts.Add(new ClaimPart
                    {
                        ClaimPartId = Guid.NewGuid(),
                        ClaimId = c.ClaimId,
                        Model = PickInstalledModel(veh.Vin),
                        SerialNumberOld = vehiclePartHistories.FirstOrDefault(vp => vp.Vin == veh.Vin)?.SerialNumber ?? "UNKNOWN",
                        SerialNumberNew = claimPartStatus == ClaimPartStatus.Enough.GetClaimPartStatus() ? $"SN{Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper()}" : null,
                        Action = ClaimPartAction.Replace.GetClaimPartAction(),
                        Status = claimPartStatus,
                        Cost = rng.Next(100, 1000)
                    });
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

            // === Attachments, BackClaims, Appointments === (unchanged)
            // reuse earlier scEmployeeUserIds list
            var attachments = new Faker<ClaimAttachment>("en")
                .RuleFor(a => a.AttachmentId, f => f.Database.Random.Guid().ToString())
                .RuleFor(a => a.ClaimId, f => f.PickRandom(claims).ClaimId)
                .RuleFor(a => a.URL, f => f.Image.PicsumUrl())
                .RuleFor(a => a.UploadedBy, f => f.PickRandom(scEmployeeUserIds))
                .Generate(recordCount);
            context.ClaimAttachments.AddRange(attachments);
            var backClaims = new Faker<BackWarrantyClaim>("en")
                .RuleFor(b => b.WarrantyClaimId, f => f.PickRandom(claims).ClaimId)
                .RuleFor(b => b.CreatedDate, f => f.Date.Recent(30))
                .RuleFor(b => b.Description, f => f.Lorem.Sentence(6))
                .RuleFor(b => b.CreatedByEmployeeId, f => f.PickRandom(scEmployeeUserIds))
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

            // === Report ===
            try
            {
                Console.WriteLine("==== Seed Report (Adjusted) ====");
                Console.WriteLine($"Total Organizations: {organizations.Count}");
                Console.WriteLine($"Total Employees: {employees.Count}");
                Console.WriteLine($"Total Customers: {customers.Count}");
                Console.WriteLine($"Total Vehicles: {vehicles.Count}");
                Console.WriteLine($"Total Parts: {parts.Count}");
                Console.WriteLine($"Total Campaigns: {campaigns.Count}");
                Console.WriteLine($"Total VehiclePartHistories: {vehiclePartHistories.Count}");
                Console.WriteLine($"New InStock: {vehiclePartHistories.Count(vp => vp.Condition == VehiclePartCondition.New.GetCondition() && vp.Status == VehiclePartCurrentStatus.InStock.GetCurrentStatus())}");
                Console.WriteLine($"Used OnVehicle: {vehiclePartHistories.Count(vp => vp.Condition == VehiclePartCondition.Used.GetCondition() && vp.Status == VehiclePartCurrentStatus.OnVehicle.GetCurrentStatus())}");
                foreach (var line in reportLines) Console.WriteLine(line);
                var path = Path.Combine(AppContext.BaseDirectory, "SeedReport.txt");
                File.WriteAllLines(path, reportLines);
            }
            catch { }
        }
    }
}