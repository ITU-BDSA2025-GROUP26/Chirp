using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Razor.Areas.Identity.Data;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Core;
using Chirp.Core.Models;

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

        public async Task OnGetAsync(int? page = 1)
        {
            CurrentUser = await _userManager.GetUserAsync(User);

            if (CurrentUser != null)
            {
                const int pageSize = 32;
                int currentPage = page ?? 1;

                Cheeps = _service.GetCheepsFromAuthor(CurrentUser.UserName, currentPage, pageSize);
                ViewData["CurrentPage"] = currentPage;
            }
        }
    }
}