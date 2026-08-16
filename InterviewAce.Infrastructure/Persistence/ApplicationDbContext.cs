using InterviewAce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace InterviewAce.Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {

    }


    public DbSet<User> Users { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    public DbSet<CandidateProfile> CandidateProfiles { get; set; }

    public DbSet<Resume> Resumes { get; set; }

    public DbSet<JobDescription> JobDescriptions { get; set; }

    public DbSet<ResumeAnalysis> ResumeAnalyses { get; set; }

    public DbSet<JobMatchAnalysis> JobMatchAnalyses { get; set; }

    public DbSet<Interview> Interviews { get; set; }

    public DbSet<InterviewQuestion> InterviewQuestions { get; set; }

    public DbSet<InterviewSession> InterviewSessions { get; set; }

    public DbSet<InterviewAnswer> InterviewAnswers { get; set; }

    public DbSet<AnswerEvaluation> AnswerEvaluations { get; set; }





    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        // Refresh Token Configuration
        modelBuilder.Entity<RefreshToken>()
            .ToTable("RefreshTokens");


        modelBuilder.Entity<RefreshToken>()
            .HasKey(x => x.Id);


        modelBuilder.Entity<RefreshToken>()
            .HasOne(x => x.User)
            .WithMany(x => x.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);



        // Candidate Profile Configuration

        modelBuilder.Entity<CandidateProfile>()
            .HasOne(x => x.User)
            .WithOne(x => x.CandidateProfile)
            .HasForeignKey<CandidateProfile>(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);



        // Resume Configuration

        modelBuilder.Entity<Resume>()
            .HasKey(x => x.Id);


        modelBuilder.Entity<Resume>()
            .Property(x => x.FileName)
            .IsRequired()
            .HasMaxLength(255);


        modelBuilder.Entity<Resume>()
            .Property(x => x.FilePath)
            .IsRequired()
            .HasMaxLength(500);


        modelBuilder.Entity<Resume>()
            .Property(x => x.FileType)
            .IsRequired()
            .HasMaxLength(100);


        modelBuilder.Entity<Resume>()
            .HasOne(x => x.User)
            .WithMany(x => x.Resumes)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobDescription>()
    .HasOne(x => x.User)
    .WithMany(x => x.JobDescriptions)
    .HasForeignKey(x => x.UserId)
    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<ResumeAnalysis>()
    .HasOne(x => x.Resume)
    .WithMany(x => x.ResumeAnalyses)
    .HasForeignKey(x => x.ResumeId)
    .OnDelete(DeleteBehavior.Cascade);


        // Job Match Analysis Configuration

        modelBuilder.Entity<JobMatchAnalysis>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<JobMatchAnalysis>()
            .HasOne(x => x.ResumeAnalysis)
            .WithMany()
            .HasForeignKey(x => x.ResumeAnalysisId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<JobMatchAnalysis>()
            .HasOne(x => x.JobDescription)
            .WithMany()
            .HasForeignKey(x => x.JobDescriptionId)
            .OnDelete(DeleteBehavior.Cascade);

       
        
        modelBuilder.Entity<Interview>(entity =>
        {
            entity.HasKey(x => x.Id);

            entity.HasOne(x => x.User)
                .WithMany()
                .HasForeignKey(x => x.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(x => x.ResumeAnalysis)
                .WithMany()
                .HasForeignKey(x => x.ResumeAnalysisId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(x => x.JobDescription)
                .WithMany()
                .HasForeignKey(x => x.JobDescriptionId)
                .OnDelete(DeleteBehavior.Restrict);

            entity.HasMany(x => x.Questions)
                .WithOne(x => x.Interview)
                .HasForeignKey(x => x.InterviewId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<InterviewQuestion>(entity =>
        {
            entity.HasKey(x => x.Id);
        });

        modelBuilder.Entity<InterviewSession>()
    .HasOne(x => x.Interview)
    .WithMany()
    .HasForeignKey(x => x.InterviewId)
    .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InterviewSession>()
            .HasOne(x => x.User)
            .WithMany()
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InterviewAnswer>()
            .HasOne(x => x.InterviewSession)
            .WithMany(x => x.Answers)
            .HasForeignKey(x => x.InterviewSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<InterviewAnswer>()
            .HasOne(x => x.InterviewQuestion)
            .WithMany()
            .HasForeignKey(x => x.InterviewQuestionId)
            .OnDelete(DeleteBehavior.Restrict);


        modelBuilder.Entity<InterviewAnswer>(entity =>
        {
            entity.HasKey(a => a.Id);

            entity.Property(a => a.AnswerText)
                .IsRequired();

            entity.Property(a => a.AnsweredAt)
                .IsRequired();

            entity.HasOne(a => a.InterviewSession)
                .WithMany(s => s.Answers)
                .HasForeignKey(a => a.InterviewSessionId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(a => a.InterviewQuestion)
                .WithMany()
                .HasForeignKey(a => a.InterviewQuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        });


        modelBuilder.Entity<AnswerEvaluation>()
    .HasOne(x => x.InterviewAnswer)
    .WithMany()
    .HasForeignKey(x => x.InterviewAnswerId)
    .OnDelete(DeleteBehavior.Cascade);
    }




}