using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiGympass.Migrations
{
    public partial class NamedTableGymsForgyms : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckIns_Gyms_GymId",
                table: "CheckIns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Gyms",
                table: "Gyms");

            migrationBuilder.RenameTable(
                name: "Gyms",
                newName: "gyms");

            migrationBuilder.AddPrimaryKey(
                name: "PK_gyms",
                table: "gyms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckIns_gyms_GymId",
                table: "CheckIns",
                column: "GymId",
                principalTable: "gyms",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckIns_gyms_GymId",
                table: "CheckIns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_gyms",
                table: "gyms");

            migrationBuilder.RenameTable(
                name: "gyms",
                newName: "Gyms");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Gyms",
                table: "Gyms",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckIns_Gyms_GymId",
                table: "CheckIns",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "Id");
        }
    }
}
