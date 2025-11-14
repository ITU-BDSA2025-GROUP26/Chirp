using Microsoft.AspNetCore.Identity;
namespace Chirp.Core.Models;

public class Author: IdentityUser
{
    public List<Cheep> Cheeps { get; set; } = new();
    public ICollection<Follow> Following { get; set; } = new List<Follow>();
    public ICollection<Follow> Followers { get; set; } = new List<Follow>();
}