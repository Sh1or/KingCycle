using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XEDAPVIP.Migrations
{
    /// <inheritdoc />
    // Migration để thêm khóa ngoại từ bảng Statistic tới các bảng Products, Categories và Brands
public partial class AddForeignKeysToStatisticsTable : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<int>(
            name: "ProductID",
            table: "Statistic",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "CategoryID",
            table: "Statistic",
            nullable: true);

        migrationBuilder.AddColumn<int>(
            name: "BrandID",
            table: "Statistic",
            nullable: true);

        migrationBuilder.CreateIndex(
            name: "IX_Statistic_ProductID",
            table: "Statistic",
            column: "ProductID");

        migrationBuilder.CreateIndex(
            name: "IX_Statistic_CategoryID",
            table: "Statistic",
            column: "CategoryID");

        migrationBuilder.CreateIndex(
            name: "IX_Statistic_BrandID",
            table: "Statistic",
            column: "BrandID");

        migrationBuilder.AddForeignKey(
            name: "FK_Statistic_Products_ProductID",
            table: "Statistic",
            column: "ProductID",
            principalTable: "Products",
            principalColumn: "ProductID",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Statistic_Categories_CategoryID",
            table: "Statistic",
            column: "CategoryID",
            principalTable: "Categories",
            principalColumn: "CategoryID",
            onDelete: ReferentialAction.Restrict);

        migrationBuilder.AddForeignKey(
            name: "FK_Statistic_Brands_BrandID",
            table: "Statistic",
            column: "BrandID",
            principalTable: "Brands",
            principalColumn: "BrandID",
            onDelete: ReferentialAction.Restrict);
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropForeignKey(
            name: "FK_Statistic_Products_ProductID",
            table: "Statistic");

        migrationBuilder.DropForeignKey(
            name: "FK_Statistic_Categories_CategoryID",
            table: "Statistic");

        migrationBuilder.DropForeignKey(
            name: "FK_Statistic_Brands_BrandID",
            table: "Statistic");

        migrationBuilder.DropIndex(
            name: "IX_Statistic_ProductID",
            table: "Statistic");

        migrationBuilder.DropIndex(
            name: "IX_Statistic_CategoryID",
            table: "Statistic");

        migrationBuilder.DropIndex(
            name: "IX_Statistic_BrandID",
            table: "Statistic");

        migrationBuilder.DropColumn(
            name: "ProductID",
            table: "Statistic");

        migrationBuilder.DropColumn(
            name: "CategoryID",
            table: "Statistic");

        migrationBuilder.DropColumn(
            name: "BrandID",
            table: "Statistic");
    }
}

}