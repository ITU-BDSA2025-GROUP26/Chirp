using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Chirp.Core.Models;
using Microsoft.AspNetCore.Identity;
namespace Chirp.Infrastructure;

public class ChirpDBContext : IdentityDbContext<Author>
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }
    public DbSet<Follow> Follows { get; set; }

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
        builder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany(a => a.Following)
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Follow>()
            .HasOne(f => f.Followee)
            .WithMany(a => a.Followers)
            .HasForeignKey(f => f.FolloweeId)
            .OnDelete(DeleteBehavior.Restrict);
        builder.Entity<Follow>()
            .HasIndex(f => new { f.FollowerId, f.FolloweeId})
            .IsUnique();
    }
}