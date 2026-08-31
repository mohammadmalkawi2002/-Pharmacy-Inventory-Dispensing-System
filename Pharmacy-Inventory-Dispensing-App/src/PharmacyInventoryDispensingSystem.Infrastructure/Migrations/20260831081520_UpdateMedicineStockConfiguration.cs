using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyInventoryDispensingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMedicineStockConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
       name: "CK_PrescriptionItems_QuantityDispensed_NonNegative",
       table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "QuantityDispensed",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "Unit",
                table: "Medicines");

            migrationBuilder.AddColumn<int>(
                name: "PackageUnit",
                table: "Medicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "StockUnit",
                table: "Medicines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UnitsPerPackage",
                table: "Medicines",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PackageUnit",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "StockUnit",
                table: "Medicines");

            migrationBuilder.DropColumn(
                name: "UnitsPerPackage",
                table: "Medicines");

            migrationBuilder.AddColumn<int>(
                name: "QuantityDispensed",
                table: "PrescriptionItems",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Unit",
                table: "Medicines",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }
    }
}
