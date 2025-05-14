using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PROG7311_PART2_AgriEnergyConnect.Models;
using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace PROG7311_PART2_AgriEnergyConnect.Data
{
    public static class SeedData
    {
        public static async Task Initialize(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogger logger)
        {
            logger.LogInformation("Starting database seeding...");

            try
            {
                // Ensure database exists and is migrated
                await context.Database.EnsureCreatedAsync();
                logger.LogInformation("Database confirmed/created");

                // Seed Roles
                if (!await roleManager.RoleExistsAsync("Employee"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Employee"));
                    logger.LogInformation("Created Employee role");
                }

                if (!await roleManager.RoleExistsAsync("Farmer"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Farmer"));
                    logger.LogInformation("Created Farmer role");
                }

                // Seed Admin User
                var adminEmail = "admin@agrienergy.com";
                var adminUser = await userManager.FindByEmailAsync(adminEmail);

                if (adminUser == null)
                {
                    adminUser = new ApplicationUser
                    {
                        UserName = adminEmail,
                        Email = adminEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(adminUser, "Admin@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(adminUser, "Employee");
                        logger.LogInformation("Created admin user");
                    }
                    else
                    {
                        logger.LogError("Failed to create admin user");
                        foreach (var error in result.Errors)
                        {
                            logger.LogError(error.Description);
                        }
                    }
                }

                // Seed Farmer User
                var farmerEmail = "farmer@example.com";
                var farmerUser = await userManager.FindByEmailAsync(farmerEmail);

                if (farmerUser == null)
                {
                    farmerUser = new ApplicationUser
                    {
                        UserName = farmerEmail,
                        Email = farmerEmail,
                        EmailConfirmed = true
                    };

                    var result = await userManager.CreateAsync(farmerUser, "Farmer@123");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(farmerUser, "Farmer");
                        logger.LogInformation("Created farmer user");
                    }
                    else
                    {
                        logger.LogError("Failed to create farmer user");
                        foreach (var error in result.Errors)
                        {
                            logger.LogError(error.Description);
                        }
                    }
                }

                // Seed Farmers (if empty)
                if (!await context.Farmers.AnyAsync())
                {
                    var farmer1 = new Farmer
                    {
                        Name = "John Smith",
                        Email = "john@example.com",
                        ContactNumber = "0123456789"
                    };

                    var farmer2 = new Farmer
                    {
                        Name = "Sarah Johnson",
                        Email = "sarah@example.com",
                        ContactNumber = "0987654321"
                    };

                    var farmer3 = new Farmer
                    {
                        Name = "David Mkhize",
                        Email = "david@example.com",
                        ContactNumber = "0712345678"
                    };

                    await context.Farmers.AddRangeAsync(farmer1, farmer2, farmer3);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Added 3 farmers to database");
                }

                // Seed Products (if empty)
                if (!await context.Products.AnyAsync())
                {
                    var farmer1 = await context.Farmers.FirstAsync(f => f.Email == "john@example.com");
                    var farmer2 = await context.Farmers.FirstAsync(f => f.Email == "sarah@example.com");
                    var farmer3 = await context.Farmers.FirstAsync(f => f.Email == "david@example.com");

                    await context.Products.AddRangeAsync(
                        new Product { Name = "Organic Apples", Category = "Fruit", ProductionDate = DateTime.Now.AddDays(-30), FarmerId = farmer1.Id },
                        new Product { Name = "Free-range Eggs", Category = "Poultry", ProductionDate = DateTime.Now.AddDays(-15), FarmerId = farmer1.Id },
                        new Product { Name = "Organic Tomatoes", Category = "Vegetables", ProductionDate = DateTime.Now.AddDays(-20), FarmerId = farmer2.Id },
                        new Product { Name = "Grass-fed Beef", Category = "Meat", ProductionDate = DateTime.Now.AddDays(-45), FarmerId = farmer3.Id },
                        new Product { Name = "Honey", Category = "Bee Products", ProductionDate = DateTime.Now.AddDays(-10), FarmerId = farmer2.Id }
                    );

                    await context.SaveChangesAsync();
                    logger.LogInformation("Added 5 products to database");
                }

                logger.LogInformation("Database seeding completed successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
                throw;
            }
        }
    }
}