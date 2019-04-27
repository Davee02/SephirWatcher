using Microsoft.EntityFrameworkCore.Migrations;

namespace DaHo.SephirWatcher.Web.Migrations
{
    public partial class NullableTypes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "MarkWeighting",
                table: "SephirTests",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<double>(
                name: "Mark",
                table: "SephirTests",
                nullable: true,
                oldClrType: typeof(double));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "MarkWeighting",
                table: "SephirTests",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "Mark",
                table: "SephirTests",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);
        }
    }
}
