using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayerEF.Migrations
{
    /// <inheritdoc />
    public partial class AddingIsAcceptedColumnToHomeAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsAccepted",
                table: "HomeAppointments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAccepted",
                table: "HomeAppointments");
        }
    }
}
