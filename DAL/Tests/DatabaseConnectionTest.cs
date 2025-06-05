using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace DAL.Tests
{
    public class DatabaseConnectionTest
    {
        public static async Task TestConnection()
        {
            try
            {
                // Create configuration with connection string
                var configuration = new ConfigurationBuilder()
                    .AddInMemoryCollection(new Dictionary<string, string>
                    {
                        {"ConnectionStrings:DefaultConnection", "Host=localhost;Database=LabManager;Username=postgres;Password=postgres"}
                    })
                    .Build();

                // Create context
                var optionsBuilder = new DbContextOptionsBuilder<Data.ApplicationDbContext>();
                optionsBuilder.UseNpgsql(configuration.GetConnectionString("DefaultConnection"));
                
                using var context = new Data.ApplicationDbContext(optionsBuilder.Options);
                
                // Test connection by trying to create the database if it doesn't exist
                await context.Database.EnsureCreatedAsync();
                
                Console.WriteLine("Database connection successful!");
                Console.WriteLine("Database 'LabManager' is ready to use.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to database: {ex.Message}");
                throw;
            }
        }
    }
} 