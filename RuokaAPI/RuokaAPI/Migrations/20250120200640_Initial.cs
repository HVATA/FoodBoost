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
                name: "Avainsanat",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReseptiId = table.Column<int>(type: "int", nullable: false),
                    Avainsanat = table.Column<string>(type: "nvarchar(max)", nullable: false)
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
                    Etumini = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    Ainesosat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Valmistuskuvaus = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kuva1 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kuva2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kuva3 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kuva4 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kuva5 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Kuva6 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Avainsanat = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Katseluoikeus = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Reseptit", x => x.Id);
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Avainsanat");

            migrationBuilder.DropTable(
                name: "Kayttajat");

            migrationBuilder.DropTable(
                name: "Reseptit");

            migrationBuilder.DropTable(
                name: "Ruokaaineet");

            migrationBuilder.DropTable(
                name: "Suosikit");
        }
    }
}
