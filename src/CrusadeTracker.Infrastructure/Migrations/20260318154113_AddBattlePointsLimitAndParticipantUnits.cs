using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrusadeTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBattlePointsLimitAndParticipantUnits : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PointsLimit",
                table: "Battles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "BattleParticipantUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UnitNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    BattleParticipantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleParticipantUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleParticipantUnits_BattleParticipants_BattleParticipantId",
                        column: x => x.BattleParticipantId,
                        principalTable: "BattleParticipants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BattleParticipantUnits_BattleParticipantId",
                table: "BattleParticipantUnits",
                column: "BattleParticipantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleParticipantUnits");

            migrationBuilder.DropColumn(
                name: "PointsLimit",
                table: "Battles");
        }
    }
}
