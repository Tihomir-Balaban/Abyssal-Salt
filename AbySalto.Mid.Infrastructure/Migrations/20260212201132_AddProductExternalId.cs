using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AbySalto.Mid.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddProductExternalId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExternalId",
                table: "Products",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Products_ExternalId",
                table: "Products",
                column: "ExternalId",
                unique: true,
                filter: "[ExternalId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Products_ExternalId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Products");
        }
    }
}
