using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyInventoryDispensingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDispensingConcurrencyTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "PrescriptionItems",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);

            migrationBuilder.AddColumn<byte[]>(
                name: "RowVersion",
                table: "Medicines",
                type: "rowversion",
                rowVersion: true,
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "RowVersion",
                table: "Medicines");
        }
    }
}
