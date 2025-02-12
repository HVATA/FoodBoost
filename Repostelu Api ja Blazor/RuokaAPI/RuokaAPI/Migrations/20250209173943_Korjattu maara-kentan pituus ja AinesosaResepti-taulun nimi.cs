using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuokaAPI.Migrations
{
    /// <inheritdoc />
    public partial class KorjattumaarakentanpituusjaAinesosaReseptitaulunnimi : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AineosaResepti_Ainesosat_AinesosaId",
                table: "AineosaResepti");

            migrationBuilder.DropForeignKey(
                name: "FK_AineosaResepti_Reseptit_ReseptiId",
                table: "AineosaResepti");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AineosaResepti",
                table: "AineosaResepti");

            migrationBuilder.RenameTable(
                name: "AineosaResepti",
                newName: "AinesosaResepti");

            migrationBuilder.RenameIndex(
                name: "IX_AineosaResepti_AinesosaId",
                table: "AinesosaResepti",
                newName: "IX_AinesosaResepti_AinesosaId");

            migrationBuilder.AlterColumn<string>(
                name: "Maara",
                table: "AinesosaResepti",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AinesosaResepti",
                table: "AinesosaResepti",
                columns: new[] { "ReseptiId", "AinesosaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AinesosaResepti_Ainesosat_AinesosaId",
                table: "AinesosaResepti",
                column: "AinesosaId",
                principalTable: "Ainesosat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AinesosaResepti_Reseptit_ReseptiId",
                table: "AinesosaResepti",
                column: "ReseptiId",
                principalTable: "Reseptit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AinesosaResepti_Ainesosat_AinesosaId",
                table: "AinesosaResepti");

            migrationBuilder.DropForeignKey(
                name: "FK_AinesosaResepti_Reseptit_ReseptiId",
                table: "AinesosaResepti");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AinesosaResepti",
                table: "AinesosaResepti");

            migrationBuilder.RenameTable(
                name: "AinesosaResepti",
                newName: "AineosaResepti");

            migrationBuilder.RenameIndex(
                name: "IX_AinesosaResepti_AinesosaId",
                table: "AineosaResepti",
                newName: "IX_AineosaResepti_AinesosaId");

            migrationBuilder.AlterColumn<string>(
                name: "Maara",
                table: "AineosaResepti",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AineosaResepti",
                table: "AineosaResepti",
                columns: new[] { "ReseptiId", "AinesosaId" });

            migrationBuilder.AddForeignKey(
                name: "FK_AineosaResepti_Ainesosat_AinesosaId",
                table: "AineosaResepti",
                column: "AinesosaId",
                principalTable: "Ainesosat",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_AineosaResepti_Reseptit_ReseptiId",
                table: "AineosaResepti",
                column: "ReseptiId",
                principalTable: "Reseptit",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
