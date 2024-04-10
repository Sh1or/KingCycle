using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace XEDAPVIP.Migrations
{
    /// <inheritdoc />
    public partial class create_table_ProductDetails : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
             migrationBuilder.CreateTable(
                name: "ProductDetails",
                columns: table => new
                {
                    ProductDetailID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductID = table.Column<int>(nullable: false),
                    Color = table.Column<string>(maxLength: 50, nullable: true),
                    Size = table.Column<string>(maxLength: 20, nullable: true),
                    FrameMaterial = table.Column<string>(maxLength: 100, nullable: true),
                    Fork = table.Column<string>(maxLength: 255, nullable: true),
                    Shock = table.Column<string>(maxLength: 100, nullable: true),
                    Rims = table.Column<string>(maxLength: 255, nullable: true),
                    Hubs = table.Column<string>(maxLength: 100, nullable: true),
                    Spokes = table.Column<string>(maxLength: 100, nullable: true),
                    Tires = table.Column<string>(maxLength: 255, nullable: true),
                    Handlebar = table.Column<string>(maxLength: 255, nullable: true),
                    Stem = table.Column<string>(maxLength: 255, nullable: true),
                    Seatpost = table.Column<string>(maxLength: 255, nullable: true),
                    Saddle = table.Column<string>(maxLength: 100, nullable: true),
                    Pedals = table.Column<string>(maxLength: 100, nullable: true),
                    Shifters = table.Column<string>(maxLength: 100, nullable: true),
                    FrontDerailleur = table.Column<string>(maxLength: 100, nullable: true),
                    RearDerailleur = table.Column<string>(maxLength: 100, nullable: true),
                    Brakes = table.Column<string>(maxLength: 100, nullable: true),
                    BrakeLevers = table.Column<string>(maxLength: 100, nullable: true),
                    Cassette = table.Column<string>(maxLength: 100, nullable: true),
                    Chain = table.Column<string>(maxLength: 100, nullable: true),
                    Crankset = table.Column<string>(maxLength: 100, nullable: true),
                    BottomBracket = table.Column<string>(maxLength: 100, nullable: true),
                    Weight = table.Column<string>(maxLength: 100, nullable: true),
                    PackingSize = table.Column<string>(maxLength: 100, nullable: true),
                    DateCreated = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()"),
                    DateUpdated = table.Column<DateTime>(nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductDetails", x => x.ProductDetailID);
                    table.ForeignKey(
                        name: "FK_ProductDetails_Products_ProductID",
                        column: x => x.ProductID,
                        principalTable: "Products",
                        principalColumn: "ProductID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ProductDetails_ProductID",
                table: "ProductDetails",
                column: "ProductID",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProductDetails");
        }
    }
}
