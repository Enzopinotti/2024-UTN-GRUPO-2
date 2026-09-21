using antigal.server.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace antigal.server.Tests;

[TestClass]
public class LikeMigrationRuntimeTests
{
    private const string PreviousMigration = "20241115051743_ActualizacionModelo";
    private const string LikeIntegrityMigration = "20260921141500_LikeConcurrencyIntegrity";

    [TestMethod]
    public void LikeIntegrityMigration_IsDiscoverableByEfRuntime()
    {
        using var context = CreateContext();

        var migrations = context.Database.GetMigrations().ToArray();

        CollectionAssert.Contains(migrations, PreviousMigration);
        CollectionAssert.Contains(migrations, LikeIntegrityMigration);
    }

    [TestMethod]
    public void LikeIntegrityMigration_GeneratesDeduplicationBeforeUniqueIndex()
    {
        using var context = CreateContext();
        var migrator = context.GetService<IMigrator>();

        var script = migrator.GenerateScript(
            PreviousMigration,
            LikeIntegrityMigration,
            MigrationsSqlGenerationOptions.Default);

        var dedupePosition = script.IndexOf("DELETE FROM RankedLikes", StringComparison.Ordinal);
        var indexPosition = script.IndexOf(
            "CREATE UNIQUE INDEX [IX_Likes_UserId_ProductoId]",
            StringComparison.Ordinal);

        Assert.IsTrue(dedupePosition >= 0, "Generated SQL is missing the duplicate cleanup.");
        Assert.IsTrue(indexPosition >= 0, "Generated SQL is missing the unique favorite index.");
        Assert.IsTrue(
            dedupePosition < indexPosition,
            "Generated SQL must deduplicate Likes before creating the unique index.");

        StringAssert.Contains(script, "PARTITION BY [UserId], [ProductoId]");
        StringAssert.Contains(script, "WHERE [UserId] IS NOT NULL");
        StringAssert.Contains(script, "WHERE [RowNumber] > 1");
    }

    private static AppDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlServer(
                "Server=localhost;Database=antigal-migration-proof;" +
                "User Id=sa;Password=NotUsedByMigrationScript123!;" +
                "TrustServerCertificate=True")
            .Options;

        return new AppDbContext(options);
    }
}
