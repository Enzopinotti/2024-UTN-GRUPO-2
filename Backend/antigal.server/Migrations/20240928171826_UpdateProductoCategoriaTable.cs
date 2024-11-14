using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace antigal.server.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductoCategoriaTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductoCategoria_Productos_ProductoidProducto",
                table: "ProductoCategoria");

            migrationBuilder.DropIndex(
                name: "IX_ProductoCategoria_ProductoidProducto",
                table: "ProductoCategoria");

            migrationBuilder.DropColumn(
                name: "ProductoidProducto",
                table: "ProductoCategoria");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductoidProducto",
                table: "ProductoCategoria",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductoCategoria_ProductoidProducto",
                table: "ProductoCategoria",
                column: "ProductoidProducto");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoCategoria_Productos_ProductoidProducto",
                table: "ProductoCategoria",
                column: "ProductoidProducto",
                principalTable: "Productos",
                principalColumn: "idProducto");
        }
    }
}
