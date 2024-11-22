using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Server.Migrations
{
    /// <inheritdoc />
    public partial class EliminarDatosCartContent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CartContent_ProductsToBuys_ProductsToBuyId",
                table: "CartContent");

            migrationBuilder.DropIndex(
                name: "IX_CartContent_ProductsToBuyId",
                table: "CartContent");

            migrationBuilder.DropColumn(
                name: "ProductsToBuyId",
                table: "CartContent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductsToBuyId",
                table: "CartContent",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_CartContent_ProductsToBuyId",
                table: "CartContent",
                column: "ProductsToBuyId");

            migrationBuilder.AddForeignKey(
                name: "FK_CartContent_ProductsToBuys_ProductsToBuyId",
                table: "CartContent",
                column: "ProductsToBuyId",
                principalTable: "ProductsToBuys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
