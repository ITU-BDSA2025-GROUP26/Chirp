using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Chirp.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Infrastructure;

public class ChirpDBContext : IdentityDbContext<Author>
{
    public DbSet<Cheep> Cheeps { get; set; } = null!;
    public DbSet<Author> Authors { get; set; } = null!;

    // NEW: table for per-user likes
    public DbSet<CheepLike> CheepLikes { get; set; } = null!;

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // --- Author configuration ---
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

        // Many-to-many: following / followers
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

        // --- NEW: CheepLike configuration (one like per (Cheep, Author)) ---
        builder.Entity<CheepLike>()
            .HasKey(cl => new { cl.CheepId, cl.AuthorId });

        builder.Entity<CheepLike>()
            .HasOne(cl => cl.Cheep)
            .WithMany()                 // no navigation collection on Cheep
            .HasForeignKey(cl => cl.CheepId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<CheepLike>()
            .HasOne(cl => cl.Author)
            .WithMany()                 // no navigation collection on Author
            .HasForeignKey(cl => cl.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
