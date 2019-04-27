using Microsoft.EntityFrameworkCore.Migrations;

namespace DaHo.SephirWatcher.Web.Migrations
{
    public partial class AddRelationships : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SephirLoginId",
                table: "SephirTests",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "IdentityUserId",
                table: "SephirLogins",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_SephirTests_SephirLoginId",
                table: "SephirTests",
                column: "SephirLoginId");

            migrationBuilder.CreateIndex(
                name: "IX_SephirLogins_IdentityUserId",
                table: "SephirLogins",
                column: "IdentityUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SephirLogins_AspNetUsers_IdentityUserId",
                table: "SephirLogins",
                column: "IdentityUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SephirTests_SephirLogins_SephirLoginId",
                table: "SephirTests",
                column: "SephirLoginId",
                principalTable: "SephirLogins",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SephirLogins_AspNetUsers_IdentityUserId",
                table: "SephirLogins");

            migrationBuilder.DropForeignKey(
                name: "FK_SephirTests_SephirLogins_SephirLoginId",
                table: "SephirTests");

            migrationBuilder.DropIndex(
                name: "IX_SephirTests_SephirLoginId",
                table: "SephirTests");

            migrationBuilder.DropIndex(
                name: "IX_SephirLogins_IdentityUserId",
                table: "SephirLogins");

            migrationBuilder.DropColumn(
                name: "SephirLoginId",
                table: "SephirTests");

            migrationBuilder.DropColumn(
                name: "IdentityUserId",
                table: "SephirLogins");
        }
    }
}
