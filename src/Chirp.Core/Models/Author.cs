using Microsoft.AspNetCore.Identity;
namespace Chirp.Core.Models;

public class Author : IdentityUser
{
    public string Name { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    public int AuthorId { get; set; }
    public List<Cheep> Cheeps { get; set; } = new();
}