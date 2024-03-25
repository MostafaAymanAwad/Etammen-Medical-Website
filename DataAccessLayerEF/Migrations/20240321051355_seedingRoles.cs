using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayerEF.Migrations
{
    /// <inheritdoc />
    public partial class seedingRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new string[] {"Id", "Name", "NormalizedName", "ConcurrencyStamp"},
                values: new object[] {Guid.NewGuid().ToString(),"Admin", "Admin".ToUpper(), Guid.NewGuid().ToString()} 
                );

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new string[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "Patient", "Patient".ToUpper(), Guid.NewGuid().ToString() }
                );
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new string[] { "Id", "Name", "NormalizedName", "ConcurrencyStamp" },
                values: new object[] { Guid.NewGuid().ToString(), "Doctor", "Doctor".ToUpper(), Guid.NewGuid().ToString() }
                );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DELETE FROM [AspNetRoles]");
        }
    }
}
