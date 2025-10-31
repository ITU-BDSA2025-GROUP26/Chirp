using Microsoft.AspNetCore.Identity;
namespace Chirp.Core.Models;

public class Author: IdentityUser
{
    public List<Cheep> Cheeps { get; set; } = new();
}