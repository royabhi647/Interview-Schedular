using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Data
{
    public class ServerDataContext : DbContext
    {
        public ServerDataContext(DbContextOptions<ServerDataContext> options)
            : base(options)
        {
        }

        public DbSet<Interview> Interviews { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Interview entity configuration
            modelBuilder.Entity<Interview>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.JobTitle).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CandidateName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.CandidateEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.InterviewerName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.InterviewerEmail).IsRequired().HasMaxLength(100);
                entity.Property(e => e.StartTime).IsRequired();
                entity.Property(e => e.EndTime).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Indexes for better performance
                entity.HasIndex(e => e.CandidateEmail);
                entity.HasIndex(e => e.StartTime);
                entity.HasIndex(e => e.Status);
                entity.HasIndex(e => e.CreatedAt);
            });

            // UserToken entity configuration
            modelBuilder.Entity<UserToken>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.UserId).IsRequired().HasMaxLength(100);
                entity.Property(e => e.AccessToken).IsRequired();
                entity.Property(e => e.RefreshToken).IsRequired();
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("GETUTCDATE()");

                // Unique constraint on UserId
                entity.HasIndex(e => e.UserId).IsUnique();
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}