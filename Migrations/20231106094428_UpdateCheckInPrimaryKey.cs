using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiGympass.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCheckInPrimaryKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckIns_AspNetUsers_UserId",
                table: "CheckIns");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckIns_Gyms_GymId",
                table: "CheckIns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckIns",
                table: "CheckIns");

            migrationBuilder.AlterColumn<Guid>(
                name: "GymId",
                table: "CheckIns",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "CheckIns",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "CheckIns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckIns",
                table: "CheckIns",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_CheckIns_UserId",
                table: "CheckIns",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckIns_AspNetUsers_UserId",
                table: "CheckIns",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CheckIns_Gyms_GymId",
                table: "CheckIns",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CheckIns_AspNetUsers_UserId",
                table: "CheckIns");

            migrationBuilder.DropForeignKey(
                name: "FK_CheckIns_Gyms_GymId",
                table: "CheckIns");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CheckIns",
                table: "CheckIns");

            migrationBuilder.DropIndex(
                name: "IX_CheckIns_UserId",
                table: "CheckIns");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "CheckIns");

            migrationBuilder.AlterColumn<Guid>(
                name: "UserId",
                table: "CheckIns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "GymId",
                table: "CheckIns",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_CheckIns",
                table: "CheckIns",
                columns: new[] { "UserId", "GymId" });

            migrationBuilder.AddForeignKey(
                name: "FK_CheckIns_AspNetUsers_UserId",
                table: "CheckIns",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CheckIns_Gyms_GymId",
                table: "CheckIns",
                column: "GymId",
                principalTable: "Gyms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
