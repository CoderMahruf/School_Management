using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CrudMVC.Migrations
{
    /// <inheritdoc />
    public partial class AddClassIdToResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClassId",
                table: "Results",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Results_ClassId",
                table: "Results",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Results_Classes_ClassId",
                table: "Results",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Results_Classes_ClassId",
                table: "Results");

            migrationBuilder.DropIndex(
                name: "IX_Results_ClassId",
                table: "Results");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Results");
        }
    }
}
