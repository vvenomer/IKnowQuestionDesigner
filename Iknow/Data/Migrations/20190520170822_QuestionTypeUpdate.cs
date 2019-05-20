using Microsoft.EntityFrameworkCore.Migrations;

namespace Iknow.Data.Migrations
{
    public partial class QuestionTypeUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "question",
                table: "QuestionsTypes",
                newName: "type");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type",
                table: "QuestionsTypes",
                newName: "question");
        }
    }
}
