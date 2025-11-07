using Chirp.Core.Models;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Razor.Areas.Identity.Data;

public class ChirpUser : IdentityUser
{
    public string UserName { get; set; }
    public Author[] Follows { get; set; }
}
