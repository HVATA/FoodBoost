using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuokaAPI.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ainesosat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nimi = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ainesosat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Avainsanat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Sana = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Avainsanat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Kayttajat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Etunimi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sukunimi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Nimimerkki = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Sahkopostiosoite = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salasana = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kayttajataso = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Kayttajat", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Reseptit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Tekijäid = table.Column<int>(type: "int", nullable: false),
                    Valmistuskuvaus = table.Column<string>(type: "nvarchar(1024)", maxLength: 1024, nullable: true),
                    Kuva1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kuva2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kuva3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kuva4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kuva5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kuva6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Katseluoikeus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reseptit", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Suosikit",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    kayttajaID = table.Column<int>(type: "int", nullable: false),
                    reseptiID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Suosikit", x => x.Id);
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
                name: "Kayttajat");

            migrationBuilder.DropTable(
                name: "Suosikit");

            migrationBuilder.DropTable(
                name: "Ainesosat");

            migrationBuilder.DropTable(
                name: "Avainsanat");

            migrationBuilder.DropTable(
                name: "Reseptit");
        }
    }
}
