using Microsoft.EntityFrameworkCore;
using Reignite.Infrastructure.Data;

namespace Reignite.UnitTests.Helpers;

public static class TestDbContextFactory
{
    public static ReigniteDbContext Create(string? dbName = null)
    {
        var options = new DbContextOptionsBuilder<ReigniteDbContext>()
            .UseInMemoryDatabase(databaseName: dbName ?? Guid.NewGuid().ToString())
            .Options;

        var context = new ReigniteDbContext(options);
        context.Database.EnsureCreated();
        return context;
    }
}
