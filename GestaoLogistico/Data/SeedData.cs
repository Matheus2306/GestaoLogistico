using GestaoLogistico.Models;
using Microsoft.AspNetCore.Identity;

namespace GestaoLogistico.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<Usuario>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles = new string[] { "Manager", "Usuário principal", "Usuario Gestor de armazem" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var adminEmail = configuration["AdminUser:Email"] 
                ?? throw new InvalidOperationException("AdminUser:Email não configurado.");
            var adminPassword = configuration["AdminUser:Password"] 
                ?? throw new InvalidOperationException("AdminUser:Password não configurado.");
            var adminName = configuration["AdminUser:Name"] 
                ?? throw new InvalidOperationException("AdminUser:Name não configurado.");
            var adminCpf = configuration["AdminUser:CPF"] ?? "00000000000";

            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var adminUser = new Usuario
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    NomeCompleto = adminName,
                    CPF = adminCpf,
                    EmailConfirmed = true,
                    CriadoEm = DateTime.UtcNow
                };
                
                var result = await userManager.CreateAsync(adminUser, adminPassword);
                
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Manager");
                }
                else
                {
                    throw new InvalidOperationException(
                        $"Erro ao criar usuário admin: {string.Join(", ", result.Errors.Select(e => e.Description))}");
                }
            }
        }
    }
}
