using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrusadeTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitBattlefieldRoleAndEquipment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BattlefieldRole",
                table: "CrusadeUnits",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Equipment",
                table: "CrusadeUnits",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BattlefieldRole",
                table: "CrusadeUnits");

            migrationBuilder.DropColumn(
                name: "Equipment",
                table: "CrusadeUnits");
        }
    }
}
