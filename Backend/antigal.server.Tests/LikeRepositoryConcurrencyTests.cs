using antigal.server.Data;
using antigal.server.Models;
using antigal.server.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace antigal.server.Tests;

[TestClass]
public class LikeRepositoryConcurrencyTests
{
    [TestMethod]
    public void Model_HasUniqueFilteredUserProductIndex()
    {
        using var connection = OpenConnection();
        using var context = CreateContext(connection);

        var entity = context.Model.FindEntityType(typeof(Like));
        Assert.IsNotNull(entity);

        var index = entity.GetIndexes().Single(candidate =>
            candidate.Properties.Select(property => property.Name)
                .SequenceEqual(new[] { nameof(Like.UserId), nameof(Like.ProductoId) }));

        Assert.IsTrue(index.IsUnique);
        Assert.AreEqual("[UserId] IS NOT NULL", index.GetFilter());
    }

    [TestMethod]
    public async Task AddLikeAsync_ExistingPair_ReturnsFalseAfterUniqueConflict()
    {
        using var connection = OpenConnection();
        await using var context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();
        await SeedUserAsync(context, "user-duplicate");

        var repository = new LikeRepository(context);

        var first = await repository.AddLikeAsync("user-duplicate", 10);
        var second = await repository.AddLikeAsync("user-duplicate", 10);

        Assert.IsTrue(first);
        Assert.IsFalse(second);
        Assert.AreEqual(
            1,
            await context.Likes.CountAsync(l =>
                l.UserId == "user-duplicate" && l.ProductoId == 10));
    }

    [TestMethod]
    public async Task AddLikeAsync_UnrelatedDatabaseFailure_IsRethrown()
    {
        using var connection = OpenConnection();
        await using var context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();

        var repository = new LikeRepository(context);

        await Assert.ThrowsExactlyAsync<DbUpdateException>(() =>
            repository.AddLikeAsync("missing-user", 20));

        Assert.AreEqual(0, await context.Likes.CountAsync());
    }

    [TestMethod]
    public async Task UniqueFilter_PreservesHistoricalNullUserRows()
    {
        using var connection = OpenConnection();
        await using var context = CreateContext(connection);
        await context.Database.EnsureCreatedAsync();

        context.Likes.AddRange(
            new Like { UserId = null, ProductoId = 30 },
            new Like { UserId = null, ProductoId = 30 });

        await context.SaveChangesAsync();

        Assert.AreEqual(
            2,
            await context.Likes.CountAsync(l =>
                l.UserId == null && l.ProductoId == 30));
    }

    private static SqliteConnection OpenConnection()
    {
        var connection = new SqliteConnection("Data Source=:memory:;Foreign Keys=True");
        connection.Open();
        return connection;
    }

    private static AppDbContext CreateContext(SqliteConnection connection)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        return new AppDbContext(options);
    }

    private static async Task SeedUserAsync(AppDbContext context, string userId)
    {
        context.Users.Add(new User
        {
            Id = userId,
            UserName = userId,
            NormalizedUserName = userId.ToUpperInvariant()
        });

        await context.SaveChangesAsync();
    }
}
