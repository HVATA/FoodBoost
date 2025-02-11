using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuokaAPI.Migrations
{
    /// <inheritdoc />
    public partial class lisättysuhdereseptiarvostelu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Arvostelut_ReseptiId",
                table: "Arvostelut",
                column: "ReseptiId");

            migrationBuilder.AddForeignKey(
                name: "FK_Arvostelut_Reseptit_ReseptiId",
                table: "Arvostelut",
                column: "ReseptiId",
                principalTable: "Reseptit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Arvostelut_Reseptit_ReseptiId",
                table: "Arvostelut");

            migrationBuilder.DropIndex(
                name: "IX_Arvostelut_ReseptiId",
                table: "Arvostelut");
        }
    }
}
