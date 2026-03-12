using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrusadeTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Battles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Mission = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    IsFinalized = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Battles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CrusadeForces",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Faction = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PointsLimit = table.Column<int>(type: "int", nullable: false),
                    BattleIds = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrusadeForces", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BattleParticipants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ForceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Result = table.Column<int>(type: "int", nullable: false),
                    ForceNameSnapshot = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BattleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BattleParticipants", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BattleParticipants_Battles_BattleId",
                        column: x => x.BattleId,
                        principalTable: "Battles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CrusadeUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DataSheet = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    ExperiencePoints = table.Column<int>(type: "int", nullable: false),
                    ForceId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    BattleHonours = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BattleScars = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CrusadeUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CrusadeUnits_CrusadeForces_ForceId",
                        column: x => x.ForceId,
                        principalTable: "CrusadeForces",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BattleParticipants_BattleId",
                table: "BattleParticipants",
                column: "BattleId");

            migrationBuilder.CreateIndex(
                name: "IX_CrusadeUnits_ForceId",
                table: "CrusadeUnits",
                column: "ForceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BattleParticipants");

            migrationBuilder.DropTable(
                name: "CrusadeUnits");

            migrationBuilder.DropTable(
                name: "Battles");

            migrationBuilder.DropTable(
                name: "CrusadeForces");
        }
    }
}
