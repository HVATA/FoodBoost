using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RuokaAPI.Migrations
{
    /// <inheritdoc />
    public partial class lisättynimireseptiin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nimi",
                table: "Reseptit",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nimi",
                table: "Reseptit");
        }
    }
}
