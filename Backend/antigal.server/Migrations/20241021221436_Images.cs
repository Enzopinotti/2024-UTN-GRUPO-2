using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace antigal.server.Migrations
{
    /// <inheritdoc />
    public partial class Images : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductoidProducto",
                table: "Imagenes",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PublicId",
                table: "Imagenes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Imagenes_ProductoidProducto",
                table: "Imagenes",
                column: "ProductoidProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_Imagenes_Productos_ProductoidProducto",
                table: "Imagenes",
                column: "ProductoidProducto",
                principalTable: "Productos",
                principalColumn: "idProducto");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Imagenes_Productos_ProductoidProducto",
                table: "Imagenes");

            migrationBuilder.DropIndex(
                name: "IX_Imagenes_ProductoidProducto",
                table: "Imagenes");

            migrationBuilder.DropColumn(
                name: "ProductoidProducto",
                table: "Imagenes");

            migrationBuilder.DropColumn(
                name: "PublicId",
                table: "Imagenes");
        }
    }
}
