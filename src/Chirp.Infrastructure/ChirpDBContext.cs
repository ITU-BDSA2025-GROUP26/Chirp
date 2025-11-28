using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Chirp.Core.Models;
using Microsoft.AspNetCore.Identity;
namespace Chirp.Infrastructure;

public class ChirpDBContext : IdentityDbContext<Author>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options)
        : base(options)
    {

    }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Author>()
            .HasIndex(a => a.UserName)
            .IsUnique();
        builder.Entity<Author>()
            .HasIndex(a => a.Email)
            .IsUnique();
        builder.Entity<Author>()
            .HasMany(a => a.Cheeps)
            .WithOne(c => c.Author)
            .HasForeignKey(c => c.AuthorId);
        builder.Entity<Cheep>()
            .HasOne(c => c.Author)
            .WithMany(a => a.Cheeps)
            .HasForeignKey(c => c.AuthorId);
        builder.Entity<Author>()
            .HasMany(a => a.Following)
            .WithMany(a => a.Followers)
            .UsingEntity<Dictionary<string, object>>(
                "AuthorFollow",
                j => j
                    .HasOne<Author>()
                    .WithMany()
                    .HasForeignKey("FolloweeId")
                    .OnDelete(DeleteBehavior.Cascade),
                j => j
                    .HasOne<Author>()
                    .WithMany()
                    .HasForeignKey("FollowerId")
                    .OnDelete(DeleteBehavior.Cascade)
            );
    }
}