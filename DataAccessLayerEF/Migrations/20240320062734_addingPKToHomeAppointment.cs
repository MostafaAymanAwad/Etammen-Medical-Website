using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayerEF.Migrations
{
    /// <inheritdoc />
    public partial class addingPKToHomeAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HomeAppointments",
                table: "HomeAppointments");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "HomeAppointments",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "HomeAppointments",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_HomeAppointments",
                table: "HomeAppointments",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_HomeAppointments_PatientId",
                table: "HomeAppointments",
                column: "PatientId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_HomeAppointments",
                table: "HomeAppointments");

            migrationBuilder.DropIndex(
                name: "IX_HomeAppointments_PatientId",
                table: "HomeAppointments");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "HomeAppointments");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "Date",
                table: "HomeAppointments",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_HomeAppointments",
                table: "HomeAppointments",
                columns: new[] { "PatientId", "DoctorId", "Date" });
        }
    }
}
