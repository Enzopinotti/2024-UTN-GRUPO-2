using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace antigal.server.Migrations
{
    /// <inheritdoc />
    public partial class FixNombresAtributosProductoCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductoCategoria_Categorias_CategoriaId",
                table: "ProductoCategoria");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoCategoria_Productos_ProductoId",
                table: "ProductoCategoria");

            migrationBuilder.RenameColumn(
                name: "CategoriaId",
                table: "ProductoCategoria",
                newName: "idCategoria");

            migrationBuilder.RenameColumn(
                name: "ProductoId",
                table: "ProductoCategoria",
                newName: "idProducto");

            migrationBuilder.RenameIndex(
                name: "IX_ProductoCategoria_CategoriaId",
                table: "ProductoCategoria",
                newName: "IX_ProductoCategoria_idCategoria");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoCategoria_Categorias_idCategoria",
                table: "ProductoCategoria",
                column: "idCategoria",
                principalTable: "Categorias",
                principalColumn: "idCategoria",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoCategoria_Productos_idProducto",
                table: "ProductoCategoria",
                column: "idProducto",
                principalTable: "Productos",
                principalColumn: "idProducto",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductoCategoria_Categorias_idCategoria",
                table: "ProductoCategoria");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductoCategoria_Productos_idProducto",
                table: "ProductoCategoria");

            migrationBuilder.RenameColumn(
                name: "idCategoria",
                table: "ProductoCategoria",
                newName: "CategoriaId");

            migrationBuilder.RenameColumn(
                name: "idProducto",
                table: "ProductoCategoria",
                newName: "ProductoId");

            migrationBuilder.RenameIndex(
                name: "IX_ProductoCategoria_idCategoria",
                table: "ProductoCategoria",
                newName: "IX_ProductoCategoria_CategoriaId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoCategoria_Categorias_CategoriaId",
                table: "ProductoCategoria",
                column: "CategoriaId",
                principalTable: "Categorias",
                principalColumn: "idCategoria",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductoCategoria_Productos_ProductoId",
                table: "ProductoCategoria",
                column: "ProductoId",
                principalTable: "Productos",
                principalColumn: "idProducto",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
