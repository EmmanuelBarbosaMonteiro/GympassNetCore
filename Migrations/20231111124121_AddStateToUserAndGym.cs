using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ApiGympass.Migrations
{
    /// <inheritdoc />
    public partial class AddStateToUserAndGym : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "state",
                table: "Gyms",
                newName: "State");

            migrationBuilder.RenameColumn(
                name: "state",
                table: "AspNetUsers",
                newName: "State");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "State",
                table: "Gyms",
                newName: "state");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "AspNetUsers",
                newName: "state");
        }
    }
}
