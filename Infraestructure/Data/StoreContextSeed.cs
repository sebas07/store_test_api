using Core.Entities;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infraestructure.Data
{
    public class StoreContextSeed
    {

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
