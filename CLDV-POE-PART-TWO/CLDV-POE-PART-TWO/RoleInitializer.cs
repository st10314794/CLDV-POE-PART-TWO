using Microsoft.AspNetCore.Identity;

namespace CLDV_POE_PART_TWO
{
    public class RoleInitializer
    {

        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                // Ensure 'Admin' role exists
                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Ensure 'Customer' role exists
                if (!await roleManager.RoleExistsAsync("Client"))
                {
                    await roleManager.CreateAsync(new IdentityRole("Client"));
                }

                // Check and create admin user
                var user = await userManager.FindByEmailAsync("admin@gmail.com");
                if (user == null)
                {
                    user = new IdentityUser { UserName = "admin@gmail.com", Email = "admin@gmail.com" };
                    var result = await userManager.CreateAsync(user, "AdminMan!23");

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }
                    else
                    {
                        // Handle potential errors during user creation
                        throw new InvalidOperationException("Failed to create the admin user: " + result.Errors.FirstOrDefault()?.Description);
                    }
                }
            }
        }
    }
}
