using Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Infraestructure.Data
{
    public class StoreContextSeed
    {

        public static async Task SeedBrandsAsync(StoreContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                var seedRoute = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

                if (!context.Brands.Any())
                {
                    var brandsData = await File.ReadAllTextAsync("../Infraestructure/Data/SeedData/brands.json");
                    var brandsList = JsonSerializer.Deserialize<List<Brand>>(brandsData);
                    context.Brands.AddRange(brandsList);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(ex.Message);
            }
        }

        public static async Task SeedRolesAsync(StoreContext context, ILoggerFactory loggerFactory)
        {
            try
            {
                if (context.Roles.Any())
                    return;

                var roles = new List<Role>() {
                    new Role { Name = "Administrator" },
                    new Role { Name = "Manager" },
                    new Role { Name = "Employee" }
                };

                context.Roles.AddRange(roles);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                var logger = loggerFactory.CreateLogger<StoreContextSeed>();
                logger.LogError(ex.Message);
            }
        }

    }
}
