using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuokaAPI.Migrations
{
    /// <inheritdoc />
    public partial class lisättyMaaraAinesosaReseptitauluun : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AinesosaResepti");

            migrationBuilder.CreateTable(
                name: "AineosaResepti",
                columns: table => new
                {
                    ReseptiId = table.Column<int>(type: "int", nullable: false),
                    AinesosaId = table.Column<int>(type: "int", nullable: false),
                    Maara = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AineosaResepti", x => new { x.ReseptiId, x.AinesosaId });
                    table.ForeignKey(
                        name: "FK_AineosaResepti_Ainesosat_AinesosaId",
                        column: x => x.AinesosaId,
                        principalTable: "Ainesosat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AineosaResepti_Reseptit_ReseptiId",
                        column: x => x.ReseptiId,
                        principalTable: "Reseptit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AineosaResepti_AinesosaId",
                table: "AineosaResepti",
                column: "AinesosaId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AineosaResepti");

            migrationBuilder.CreateTable(
                name: "AinesosaResepti",
                columns: table => new
                {
                    AinesosatId = table.Column<int>(type: "int", nullable: false),
                    ReseptitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AinesosaResepti", x => new { x.AinesosatId, x.ReseptitId });
                    table.ForeignKey(
                        name: "FK_AinesosaResepti_Ainesosat_AinesosatId",
                        column: x => x.AinesosatId,
                        principalTable: "Ainesosat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AinesosaResepti_Reseptit_ReseptitId",
                        column: x => x.ReseptitId,
                        principalTable: "Reseptit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AinesosaResepti_ReseptitId",
                table: "AinesosaResepti",
                column: "ReseptitId");
        }
    }
}
