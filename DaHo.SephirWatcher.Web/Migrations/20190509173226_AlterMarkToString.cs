using Microsoft.EntityFrameworkCore.Migrations;

namespace DaHo.SephirWatcher.Web.Migrations
{
    public partial class AlterMarkToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExamState",
                table: "SephirTests");

            migrationBuilder.DropColumn(
                name: "Mark",
                table: "SephirTests");

            migrationBuilder.AddColumn<string>(
                name: "EncryptedMark",
                table: "SephirTests",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedMark",
                table: "SephirTests");

            migrationBuilder.AddColumn<string>(
                name: "ExamState",
                table: "SephirTests",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<double>(
                name: "Mark",
                table: "SephirTests",
                nullable: true);
        }
    }
}
