using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyInventoryDispensingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateMedicineConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Medicines",
                type: "nvarchar(15)",
                maxLength: 15,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddCheckConstraint(
                name: "CK_Medicines_ReorderLevel_NonNegative",
                table: "Medicines",
                sql: "[ReorderLevel] >= 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Medicines_ReorderLevel_NonNegative",
                table: "Medicines");

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Medicines",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(15)",
                oldMaxLength: 15);
        }
    }
}
