using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InterviewAce.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJobMatchAnalysis : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JobMatchAnalyses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ResumeAnalysisId = table.Column<Guid>(type: "uuid", nullable: false),
                    JobDescriptionId = table.Column<Guid>(type: "uuid", nullable: false),
                    MatchScore = table.Column<int>(type: "integer", nullable: false),
                    MatchingSkills = table.Column<string>(type: "text", nullable: false),
                    MissingSkills = table.Column<string>(type: "text", nullable: false),
                    ExperienceMatch = table.Column<string>(type: "text", nullable: false),
                    Strengths = table.Column<string>(type: "text", nullable: false),
                    Gaps = table.Column<string>(type: "text", nullable: false),
                    Recommendations = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobMatchAnalyses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JobMatchAnalyses_JobDescriptions_JobDescriptionId",
                        column: x => x.JobDescriptionId,
                        principalTable: "JobDescriptions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_JobMatchAnalyses_ResumeAnalyses_ResumeAnalysisId",
                        column: x => x.ResumeAnalysisId,
                        principalTable: "ResumeAnalyses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JobMatchAnalyses_JobDescriptionId",
                table: "JobMatchAnalyses",
                column: "JobDescriptionId");

            migrationBuilder.CreateIndex(
                name: "IX_JobMatchAnalyses_ResumeAnalysisId",
                table: "JobMatchAnalyses",
                column: "ResumeAnalysisId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JobMatchAnalyses");
        }
    }
}
