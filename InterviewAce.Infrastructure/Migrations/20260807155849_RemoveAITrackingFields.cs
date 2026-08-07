using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewAce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAITrackingFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AIModel",
                table: "ResumeAnalyses");

            migrationBuilder.DropColumn(
                name: "PromptVersion",
                table: "ResumeAnalyses");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AIModel",
                table: "ResumeAnalyses",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "PromptVersion",
                table: "ResumeAnalyses",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
