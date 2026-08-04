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
    }
}