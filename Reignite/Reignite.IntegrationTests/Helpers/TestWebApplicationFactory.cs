using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Reignite.Infrastructure.Data;

namespace Reignite.IntegrationTests.Helpers;

public class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string _dbName = "IntegrationTestDb_" + Guid.NewGuid().ToString();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        // Set required environment variables BEFORE the app starts
        Environment.SetEnvironmentVariable("DB_SERVER", "localhost");
        Environment.SetEnvironmentVariable("DB_PORT", "1433");
        Environment.SetEnvironmentVariable("DB_NAME", "TestDb");
        Environment.SetEnvironmentVariable("DB_USER", "sa");
        Environment.SetEnvironmentVariable("DB_PASSWORD", "test");
        Environment.SetEnvironmentVariable("JWT_SECRET", "ThisIsAVeryLongTestSecretKeyForIntegrationTests123456!");
        Environment.SetEnvironmentVariable("JWT_ISSUER", "ReigniteTest");
        Environment.SetEnvironmentVariable("JWT_AUDIENCE", "ReigniteTestApp");
        Environment.SetEnvironmentVariable("STRIPE_SECRET_KEY", "sk_test_fake_key_for_testing");
        Environment.SetEnvironmentVariable("STRIPE_PUBLISHABLE_KEY", "pk_test_fake_key_for_testing");

        builder.UseEnvironment("Testing");

        builder.ConfigureServices(services =>
        {
            // Remove the real SQL Server DbContext registration
            var descriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(DbContextOptions<ReigniteDbContext>));
            if (descriptor != null) services.Remove(descriptor);

            // Also remove any DbContext registration
            var contextDescriptor = services.SingleOrDefault(
                d => d.ServiceType == typeof(ReigniteDbContext));
            if (contextDescriptor != null) services.Remove(contextDescriptor);

            // Add InMemory database with a FIXED name per factory instance
            var dbName = _dbName;
            services.AddDbContext<ReigniteDbContext>(options =>
            {
                options.UseInMemoryDatabase(dbName);
            });

            // Ensure the database is created
            var sp = services.BuildServiceProvider();
            using var scope = sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ReigniteDbContext>();
            db.Database.EnsureCreated();
        });
    }
}
