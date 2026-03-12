using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ged_fdl_razor.Migrations
{
    /// <inheritdoc />
    public partial class AddDocumentEstVerifie : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "EstVerifie",
                table: "Documents",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EstVerifie",
                table: "Documents");
        }
    }
}
