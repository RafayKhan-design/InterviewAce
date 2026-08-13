using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewAce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Interviews",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobDescriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    InterviewType = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<string>(type: "text", nullable: false),
                    QuestionCount = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Interviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Interviews_JobDescriptions_JobDescriptionId",
                        column: x => x.JobDescriptionId,
                        principalTable: "JobDescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interviews_ResumeAnalyses_ResumeAnalysisId",
                        column: x => x.ResumeAnalysisId,
                        principalTable: "ResumeAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Interviews_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InterviewQuestions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    InterviewId = table.Column<Guid>(type: "uuid", nullable: false),
                    Order = table.Column<int>(type: "integer", nullable: false),
                    Question = table.Column<string>(type: "text", nullable: false),
                    QuestionType = table.Column<string>(type: "text", nullable: false),
                    ExpectedTopics = table.Column<string>(type: "text", nullable: false),
                    Difficulty = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewQuestions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewQuestions_Interviews_InterviewId",
                        column: x => x.InterviewId,
                        principalTable: "Interviews",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewQuestions_InterviewId",
                table: "InterviewQuestions",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_JobDescriptionId",
                table: "Interviews",
                column: "JobDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_ResumeAnalysisId",
                table: "Interviews",
                column: "ResumeAnalysisId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_UserId",
                table: "Interviews",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewQuestions");

            migrationBuilder.DropTable(
                name: "Interviews");
        }
    }
}
