using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using OrderHub.Adapters.Outbound.Persistence;

namespace OrderHub.Api.IntegrationTests.Fixtures;

/// <summary>
/// Custom WebApplicationFactory for integration tests.
/// Configures the test environment with an in-memory database instead of SQL Server.
/// </summary>
public class OrderHubWebApplicationFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            // Remove the existing OrderHubDbContext service
            var descriptor = services.SingleOrDefault(d =>
                d.ServiceType == typeof(DbContextOptions<OrderHubDbContext>));
            
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }

            // Add in-memory database for testing
            services.AddDbContext<OrderHubDbContext>(options =>
            {
                options.UseInMemoryDatabase("OrderHubTest");
            });

            // Build the service provider and ensure database is created
            var serviceProvider = services.BuildServiceProvider();
            
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<OrderHubDbContext>();
            
            // Ensure database is created and clean
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();
        });

        // Disable HTTPS redirection for test environment
        builder.ConfigureServices(services =>
        {
            services.PostConfigure<Microsoft.AspNetCore.HttpsPolicy.HttpsRedirectionOptions>(options =>
            {
                options.HttpsPort = null;
            });
        });
    }

    /// <summary>
    /// Factory method to create a new DbContext instance for test assertions.
    /// Useful for verifying database state after API calls.
    /// </summary>
    public OrderHubDbContext CreateDbContext()
    {
        var scope = Services.CreateScope();
        return scope.ServiceProvider.GetRequiredService<OrderHubDbContext>();
    }
}
