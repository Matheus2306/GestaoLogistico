using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace GestaoLogistico.Data
{
    /// <summary>
    /// Factory para criar o DbContext em tempo de design (migrações, scaffolding, etc.)
    /// </summary>
    public class AplicationDbContextFactory : IDesignTimeDbContextFactory<AplicationDbContext>
    {
        public AplicationDbContext CreateDbContext(string[] args)
        {
            // Configuração para encontrar o appsettings.json
            var configurationBuilder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);

            // ✅ Adicionar User Secrets apenas se o arquivo existir
            try
            {
                configurationBuilder.AddUserSecrets<Program>(optional: true);
            }
            catch
            {
                // Ignora erros de User Secrets em tempo de design
            }

            // Adiciona variáveis de ambiente por último (maior prioridade)
            configurationBuilder.AddEnvironmentVariables();

            var configuration = configurationBuilder.Build();

            // Obtém a connection string
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Connection string 'DefaultConnection' não encontrada. " +
                    "Certifique-se de que está definida em appsettings.json, User Secrets ou variáveis de ambiente.");
            }

            // Configura o DbContext
            var optionsBuilder = new DbContextOptionsBuilder<AplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.CommandTimeout(120);
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(10),
                    errorNumbersToAdd: null);
            });

            return new AplicationDbContext(optionsBuilder.Options);
        }
    }
}