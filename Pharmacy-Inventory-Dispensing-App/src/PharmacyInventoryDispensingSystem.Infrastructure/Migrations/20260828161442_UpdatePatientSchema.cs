using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PharmacyInventoryDispensingSystem.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePatientSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Patients");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Patients",
                type: "nvarchar(16)",
                maxLength: 16,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Patients",
                type: "date",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "FullName",
                table: "Patients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_CreatedAtUtc_Id",
                table: "Patients",
                columns: new[] { "CreatedAtUtc", "Id" },
                filter: "[IsDeleted] = 0");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_FullName_Id",
                table: "Patients",
                columns: new[] { "FullName", "Id" },
                filter: "[IsDeleted] = 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Patients_CreatedAtUtc_Id",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_FullName_Id",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "FullName",
                table: "Patients");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Patients",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(16)",
                oldMaxLength: 16);

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateOfBirth",
                table: "Patients",
                type: "datetime2",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "date");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Patients",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Patients",
                type: "nvarchar(40)",
                maxLength: 40,
                nullable: false,
                defaultValue: "");
        }
    }
}
