using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayerEF.Migrations
{
    /// <inheritdoc />
    public partial class AddingIsDelted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateOnly>(
                name: "CreationDate",
                table: "DoctorReviews",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<DateOnly>(
                name: "DeletionDate",
                table: "DoctorReviews",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "DoctorReviews",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateOnly>(
                name: "LastUpdate",
                table: "DoctorReviews",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreationDate",
                table: "DoctorReviews");

            migrationBuilder.DropColumn(
                name: "DeletionDate",
                table: "DoctorReviews");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "DoctorReviews");

            migrationBuilder.DropColumn(
                name: "LastUpdate",
                table: "DoctorReviews");
        }
    }
}
