using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Razor.Areas.Identity.Data;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Core;
using Chirp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Razor.Pages
{
    public class AboutMeModel : PageModel
    {
        private readonly UserManager<Author> _userManager;
        private readonly ICheepService _service;

        public AboutMeModel(UserManager<Author> userManager, ICheepService service)
        {
            _userManager = userManager;
            _service = service;
        }

        public Author CurrentUser { get; set; }
        public List<CheepDto> Cheeps { get; set; } = new();
        
        public List<Author> Following { get; set; } = new();
        public List<Author> Followers { get; set; } = new();

        public async Task OnGetAsync(int? page = 1)
        {
            
            var user = await _userManager.GetUserAsync(User);

            if (user != null)
            {
                CurrentUser = await _userManager.Users
                    .Include(a => a.Following)
                    .Include(a => a.Followers)
                    .FirstOrDefaultAsync(u => u.Id == user.Id);

                if (CurrentUser != null)
                {
                    const int pageSize = 32;
                    int currentPage = page ?? 1;

                    Cheeps = _service.GetCheepsFromAuthor(CurrentUser.UserName, currentPage, pageSize);
                    ViewData["CurrentPage"] = currentPage;
                    
                    Following = CurrentUser.Following.ToList();
                    Followers = CurrentUser.Followers.ToList();
                }
            }
        }
         public async Task<IActionResult> OnPostUnfollow(string authorToUnfollow, string author)
                {
                    if (!(User?.Identity?.IsAuthenticated ?? false))
                        return Unauthorized();
        
                    var follower = User.Identity!.Name!;
                    await _service.Unfollow(follower, authorToUnfollow);
                    
                    return RedirectToPage("/AboutMe", new { author = author });
                }
    }
}