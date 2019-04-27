using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DaHo.SephirWatcher.Web.Migrations
{
    public partial class AddSephirTests : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SephirTests",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExamDate = table.Column<DateTime>(nullable: false),
                    SchoolSubject = table.Column<string>(nullable: false),
                    ExamTitle = table.Column<string>(nullable: false),
                    ExamState = table.Column<string>(nullable: false),
                    MarkType = table.Column<string>(nullable: false),
                    MarkWeighting = table.Column<double>(nullable: false),
                    Mark = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SephirTests", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SephirTests");
        }
    }
}
