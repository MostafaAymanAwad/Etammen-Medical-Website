using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayerEF.Migrations
{
    /// <inheritdoc />
    public partial class AddingIDtoDoctorAndpatientTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_PatientApplicationUserId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Doctors_Id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Patients_Id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Clinics_Doctors_DoctorApplicationUserId",
                table: "Clinics");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorReviews_Doctors_DoctorApplicationUserId",
                table: "DoctorReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorReviews_Patients_PatientApplicationUserId",
                table: "DoctorReviews");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_DoctorReviews_DoctorApplicationUserId",
                table: "DoctorReviews");

            migrationBuilder.DropIndex(
                name: "IX_DoctorReviews_PatientApplicationUserId",
                table: "DoctorReviews");

            migrationBuilder.DropIndex(
                name: "IX_Clinics_DoctorApplicationUserId",
                table: "Clinics");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_PatientApplicationUserId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "DoctorApplicationUserId",
                table: "DoctorReviews");

            migrationBuilder.DropColumn(
                name: "PatientApplicationUserId",
                table: "DoctorReviews");

            migrationBuilder.DropColumn(
                name: "DoctorApplicationUserId",
                table: "Clinics");

            migrationBuilder.DropColumn(
                name: "PatientApplicationUserId",
                table: "Appointments");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Patients",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Doctors",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Doctors",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Patients_ApplicationUserId",
                table: "Patients",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Doctors_ApplicationUserId",
                table: "Doctors",
                column: "ApplicationUserId",
                unique: true,
                filter: "[ApplicationUserId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorReviews_DoctorId",
                table: "DoctorReviews",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_DoctorId",
                table: "Clinics",
                column: "DoctorId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_patientId",
                table: "Appointments",
                column: "patientId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_patientId",
                table: "Appointments",
                column: "patientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clinics_Doctors_DoctorId",
                table: "Clinics",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorReviews_Doctors_DoctorId",
                table: "DoctorReviews",
                column: "DoctorId",
                principalTable: "Doctors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorReviews_Patients_PatientId",
                table: "DoctorReviews",
                column: "PatientId",
                principalTable: "Patients",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Doctors_AspNetUsers_ApplicationUserId",
                table: "Doctors",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Patients_AspNetUsers_ApplicationUserId",
                table: "Patients",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Patients_patientId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_Clinics_Doctors_DoctorId",
                table: "Clinics");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorReviews_Doctors_DoctorId",
                table: "DoctorReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_DoctorReviews_Patients_PatientId",
                table: "DoctorReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Doctors_AspNetUsers_ApplicationUserId",
                table: "Doctors");

            migrationBuilder.DropForeignKey(
                name: "FK_Patients_AspNetUsers_ApplicationUserId",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Patients",
                table: "Patients");

            migrationBuilder.DropIndex(
                name: "IX_Patients_ApplicationUserId",
                table: "Patients");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_Doctors_ApplicationUserId",
                table: "Doctors");

            migrationBuilder.DropIndex(
                name: "IX_DoctorReviews_DoctorId",
                table: "DoctorReviews");

            migrationBuilder.DropIndex(
                name: "IX_Clinics_DoctorId",
                table: "Clinics");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_patientId",
                table: "Appointments");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Patients");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Doctors");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Patients",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "Doctors",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DoctorApplicationUserId",
                table: "DoctorReviews",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientApplicationUserId",
                table: "DoctorReviews",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "DoctorApplicationUserId",
                table: "Clinics",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PatientApplicationUserId",
                table: "Appointments",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Patients",
                table: "Patients",
                column: "ApplicationUserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Doctors",
                table: "Doctors",
                column: "ApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorReviews_DoctorApplicationUserId",
                table: "DoctorReviews",
                column: "DoctorApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_DoctorReviews_PatientApplicationUserId",
                table: "DoctorReviews",
                column: "PatientApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Clinics_DoctorApplicationUserId",
                table: "Clinics",
                column: "DoctorApplicationUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_PatientApplicationUserId",
                table: "Appointments",
                column: "PatientApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Patients_PatientApplicationUserId",
                table: "Appointments",
                column: "PatientApplicationUserId",
                principalTable: "Patients",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Doctors_Id",
                table: "AspNetUsers",
                column: "Id",
                principalTable: "Doctors",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Patients_Id",
                table: "AspNetUsers",
                column: "Id",
                principalTable: "Patients",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Clinics_Doctors_DoctorApplicationUserId",
                table: "Clinics",
                column: "DoctorApplicationUserId",
                principalTable: "Doctors",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorReviews_Doctors_DoctorApplicationUserId",
                table: "DoctorReviews",
                column: "DoctorApplicationUserId",
                principalTable: "Doctors",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorReviews_Patients_PatientApplicationUserId",
                table: "DoctorReviews",
                column: "PatientApplicationUserId",
                principalTable: "Patients",
                principalColumn: "ApplicationUserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
