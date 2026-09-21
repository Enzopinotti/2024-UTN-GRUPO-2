using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace antigal.server.Migrations
{
    /// <inheritdoc />
    public partial class LikeConcurrencyIntegrity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Historical databases may already contain duplicate authenticated favorites.
            // Keep the oldest row per (UserId, ProductoId) before creating the unique index.
            migrationBuilder.Sql(
                """
                ;WITH RankedLikes AS
                (
                    SELECT
                        [Id],
                        ROW_NUMBER() OVER (
                            PARTITION BY [UserId], [ProductoId]
                            ORDER BY [Id]
                        ) AS [RowNumber]
                    FROM [Likes]
                    WHERE [UserId] IS NOT NULL
                )
                DELETE FROM RankedLikes
                WHERE [RowNumber] > 1;
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Likes_UserId_ProductoId",
                table: "Likes",
                columns: new[] { "UserId", "ProductoId" },
                unique: true,
                filter: "[UserId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Likes_UserId_ProductoId",
                table: "Likes");
        }
    }
}
