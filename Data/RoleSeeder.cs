using Microsoft.AspNetCore.Identity;

namespace SecureAgentPortal.Data;

public static class RoleSeeder
{
    public static async Task SeedAsync(IServiceProvider services, IConfiguration config)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        // 1.Seed roles
        string[] roles = ["Admin", "Agent", "Auditor"];

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        // 2.Seed Admin user
        var email =
            Environment.GetEnvironmentVariable("SEED_ADMIN_EMAIL")
            ?? config["SeedAdmin:Email"];

        var password =
            Environment.GetEnvironmentVariable("SEED_ADMIN_PASSWORD")
            ?? config["SeedAdmin:Password"];

        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            return;

        var existing = await userManager.FindByEmailAsync(email);

        if (existing is null)
        {
            var admin = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(admin, password);

            if (!result.Succeeded)
            {
                var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException("Admin seed failed: " + errors);
            }

            await userManager.AddToRoleAsync(admin, "Admin");
        }
        else
        {
            if (!await userManager.IsInRoleAsync(existing, "Admin"))
                await userManager.AddToRoleAsync(existing, "Admin");
        }
    }
}
