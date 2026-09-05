using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyInventoryDispensingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePrescriptionModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PrescriptionItems_PrescriptionId",
                table: "PrescriptionItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PrescriptionItems_MaxRefill_NonNegative",
                table: "PrescriptionItems");

            

            migrationBuilder.DropCheckConstraint(
                name: "CK_PrescriptionItems_QuantityPrescribed_Positive",
                table: "PrescriptionItems");

            migrationBuilder.DropCheckConstraint(
                name: "CK_PrescriptionItems_RefillUsed_NonNegative",
                table: "PrescriptionItems");

            migrationBuilder.DropColumn(
                name: "DeletedAtUtc",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "DeletedBy",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "Prescriptions");

            migrationBuilder.DropColumn(
                name: "RestoredAtUtc",
                table: "Prescriptions");

            migrationBuilder.RenameColumn(
        name: "MaxRefill",
        table: "PrescriptionItems",
        newName: "MaxFillCount");

            migrationBuilder.RenameColumn(
                name: "RefillUsed",
                table: "PrescriptionItems",
                newName: "FillUsedCount");

            migrationBuilder.CreateSequence<int>(
                name: "PrescriptionNumberSequence",
                maxValue: 999999L);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ValidTo",
                table: "Prescriptions",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "ValidFrom",
                table: "Prescriptions",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "PrescriptionNumber",
                table: "Prescriptions",
                type: "nvarchar(9)",
                maxLength: 9,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_PrescriptionId_MedicineId",
                table: "PrescriptionItems",
                columns: new[] { "PrescriptionId", "MedicineId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_PrescriptionItems_PrescriptionId_MedicineId",
                table: "PrescriptionItems");

            migrationBuilder.DropSequence(
                name: "PrescriptionNumberSequence");

            migrationBuilder.RenameColumn(
     name: "MaxFillCount",
     table: "PrescriptionItems",
     newName: "MaxRefill");

            migrationBuilder.RenameColumn(
                name: "FillUsedCount",
                table: "PrescriptionItems",
                newName: "RefillUsed");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidTo",
                table: "Prescriptions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ValidFrom",
                table: "Prescriptions",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<string>(
                name: "PrescriptionNumber",
                table: "Prescriptions",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(9)",
                oldMaxLength: 9);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "DeletedAtUtc",
                table: "Prescriptions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedBy",
                table: "Prescriptions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "Prescriptions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "RestoredAtUtc",
                table: "Prescriptions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_PrescriptionItems_PrescriptionId",
                table: "PrescriptionItems",
                column: "PrescriptionId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PrescriptionItems_MaxRefill_NonNegative",
                table: "PrescriptionItems",
                sql: "[MaxRefill] >= 0");


            migrationBuilder.AddCheckConstraint(
                name: "CK_PrescriptionItems_QuantityPrescribed_Positive",
                table: "PrescriptionItems",
                sql: "[QuantityPrescribed] > 0");

            migrationBuilder.AddCheckConstraint(
                name: "CK_PrescriptionItems_RefillUsed_NonNegative",
                table: "PrescriptionItems",
                sql: "[RefillUsed] >= 0");
        }
    }
}
