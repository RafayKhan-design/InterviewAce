using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewAce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AllowMultipleAnswerEvaluations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnswerEvaluations_InterviewAnswerId",
                table: "AnswerEvaluations");

            migrationBuilder.AddColumn<string>(
                name: "AIModel",
                table: "AnswerEvaluations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromptVersion",
                table: "AnswerEvaluations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerEvaluations_InterviewAnswerId",
                table: "AnswerEvaluations",
                column: "InterviewAnswerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AnswerEvaluations_InterviewAnswerId",
                table: "AnswerEvaluations");

            migrationBuilder.DropColumn(
                name: "AIModel",
                table: "AnswerEvaluations");

            migrationBuilder.DropColumn(
                name: "PromptVersion",
                table: "AnswerEvaluations");

            migrationBuilder.CreateIndex(
                name: "IX_AnswerEvaluations_InterviewAnswerId",
                table: "AnswerEvaluations",
                column: "InterviewAnswerId",
                unique: true);
        }
    }
}
