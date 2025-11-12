using Microsoft.AspNetCore.Identity;

namespace Chirp.Razor.Areas.Identity.Data;

public class ChirpUser : IdentityUser
{
    String[] FollowedUsers;
}