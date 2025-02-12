using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuokaAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddArvosteluTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Arvostelut",
                columns: table => new
                {
                    ArvosteluID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ReseptiId = table.Column<int>(type: "int", nullable: false),
                    ArvostelijanId = table.Column<int>(type: "int", nullable: false),
                    ArvostelijanNimimerkki = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Numeroarvostelu = table.Column<int>(type: "int", nullable: false),
                    Vapaateksti = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Arvostelut", x => x.ArvosteluID);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Arvostelut");
        }
    }
}
