using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Core;
using Chirp.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

namespace Chirp.Web.Pages
{
    public class AboutMeModel : PageModel
    {
        private readonly UserManager<Author> _userManager;
        private readonly ICheepService _service;
        private readonly IAuthorService _authorService;

        public AboutMeModel(UserManager<Author> userManager, ICheepService service, IAuthorService authorService)
        {
            _userManager = userManager;
            _service = service;
            _authorService = authorService;
        }

        public Author? CurrentUser { get; set; }
        public List<CheepDto> Cheeps { get; set; } = new();
        
        public List<Author> Following { get; set; } = new();
        public List<Author> Followers { get; set; } = new();

        public async Task OnGetAsync(int? page = 1)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return;
            var user = await _userManager.GetUserAsync(User);
            if (user is null)
                return;

            CurrentUser = await _userManager.Users
                .Include(a => a.Following)
                .Include(a => a.Followers)
                .FirstOrDefaultAsync(u => u.Id == user.Id);

            if (CurrentUser is null)
                return;
            
            var userName = CurrentUser.UserName;
            if (string.IsNullOrEmpty(userName))
                return;
            
            const int pageSize = 32;
            int currentPage = page ?? 1;

            Cheeps = _service.GetCheepsFromAuthor(userName, currentPage, pageSize);
            ViewData["CurrentPage"] = currentPage;
                    
            Following = CurrentUser.Following.ToList();
            Followers = CurrentUser.Followers.ToList();
        }
         public async Task<IActionResult> OnPostUnfollow(string authorToUnfollow, string author)
            {
                if (!(User?.Identity?.IsAuthenticated ?? false))
                    return Unauthorized();
    
                var follower = User.Identity!.Name!;
                await _authorService.Unfollow(follower, authorToUnfollow);
                
                return RedirectToPage("/AboutMe", new { author = author });
            }
    }
}