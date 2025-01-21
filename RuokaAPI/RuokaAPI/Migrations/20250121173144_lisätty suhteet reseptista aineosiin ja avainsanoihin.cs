using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuokaAPI.Migrations
{
    /// <inheritdoc />
    public partial class lisättysuhteetreseptistaaineosiinjaavainsanoihin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Ruokaaineet");

            migrationBuilder.DropColumn(
                name: "Ainesosat",
                table: "Reseptit");

            migrationBuilder.DropColumn(
                name: "Avainsanat",
                table: "Reseptit");

            migrationBuilder.DropColumn(
                name: "ReseptiId",
                table: "Avainsanat");

            migrationBuilder.RenameColumn(
                name: "Avainsanat",
                table: "Avainsanat",
                newName: "Sana");

            migrationBuilder.CreateTable(
                name: "Ainesosat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nimi = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ainesosat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AvainsanaResepti",
                columns: table => new
                {
                    AvainsanatId = table.Column<int>(type: "int", nullable: false),
                    ReseptitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvainsanaResepti", x => new { x.AvainsanatId, x.ReseptitId });
                    table.ForeignKey(
                        name: "FK_AvainsanaResepti_Avainsanat_AvainsanatId",
                        column: x => x.AvainsanatId,
                        principalTable: "Avainsanat",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AvainsanaResepti_Reseptit_ReseptitId",
                        column: x => x.ReseptitId,
                        principalTable: "Reseptit",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

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

            migrationBuilder.CreateIndex(
                name: "IX_AvainsanaResepti_ReseptitId",
                table: "AvainsanaResepti",
                column: "ReseptitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AinesosaResepti");

            migrationBuilder.DropTable(
                name: "AvainsanaResepti");

            migrationBuilder.DropTable(
                name: "Ainesosat");

            migrationBuilder.RenameColumn(
                name: "Sana",
                table: "Avainsanat",
                newName: "Avainsanat");

            migrationBuilder.AddColumn<string>(
                name: "Ainesosat",
                table: "Reseptit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Avainsanat",
                table: "Reseptit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ReseptiId",
                table: "Avainsanat",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Ruokaaineet",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Ainesosat = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ruokaaineet", x => x.Id);
                });
        }
    }
}
