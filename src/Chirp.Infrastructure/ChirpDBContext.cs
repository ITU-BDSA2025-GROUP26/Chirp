using Microsoft.EntityFrameworkCore;
using Chirp.Core.Models;
namespace Chirp.Infrastructure;

public class ChirpDBContext : DbContext
{
    public DbSet<Cheep> Cheeps { get; set; }
    public DbSet<Author> Authors { get; set; }

    public ChirpDBContext(DbContextOptions<ChirpDBContext> options)
        : base(options)
    {
        
    }
}