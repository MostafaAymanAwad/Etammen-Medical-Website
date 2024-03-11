using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayerEF.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnIsRegisteredToDoctorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Certificate",
                table: "Doctors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsRegistered",
                table: "Doctors",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Certificate",
                table: "Doctors");

            migrationBuilder.DropColumn(
                name: "IsRegistered",
                table: "Doctors");
        }
    }
}
