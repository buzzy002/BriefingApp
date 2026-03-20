using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BriefingApp.Migrations
{
    /// <inheritdoc />
    public partial class AddNewsToggles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isBelgiumNewsWanted",
                table: "UserPreferences",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "isWorldNewsWanted",
                table: "UserPreferences",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isBelgiumNewsWanted",
                table: "UserPreferences");

            migrationBuilder.DropColumn(
                name: "isWorldNewsWanted",
                table: "UserPreferences");
        }
    }
}
