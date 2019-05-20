using Microsoft.EntityFrameworkCore.Migrations;

namespace Iknow.Data.Migrations
{
    public partial class FixQuestions : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "answer",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hint1",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hint2",
                table: "Questions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "hint3",
                table: "Questions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "answer",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "hint1",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "hint2",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "hint3",
                table: "Questions");
        }
    }
}
