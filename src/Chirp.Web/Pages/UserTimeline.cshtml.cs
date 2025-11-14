using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Core;
using Chirp.Core.Models;
using Chirp.Razor.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Razor.Pages
{
    public class UserTimelineModel : PageModel
    {
        private readonly UserManager<Author> _userManager;
        private readonly ICheepService _service;

        public Author _currentUser;
        public List<CheepDto> Cheeps { get; set; } = new();

        [BindProperty]
        public string Text { get; set; }
        public UserTimelineModel(ICheepService service,
            UserManager<Author> userManager)
        {
            _service = service;
            _userManager = userManager;
            _currentUser = _userManager?.GetUserAsync(HttpContext.User).Result;
        }

        public ActionResult OnGet(string author, [FromQuery] int? page = 1, int? pageNumber = null)
        {
            int currentPage = page ?? pageNumber ?? 1;
            const int pageSize = 32;
            
            if (_currentUser != null && _currentUser.UserName == author)
            {
                foreach(var followed in _currentUser.Following)
                {
                    Cheeps.AddRange(_service.GetCheepsFromAuthor(followed.FollowerId, currentPage, pageSize));
                }
            }
            else
            {
                Cheeps = _service.GetCheepsFromAuthor(author, currentPage, pageSize);
            }

            ViewData["CurrentPage"] = currentPage;
            ViewData["Author"] = author;
            return Page();
        }
        public IActionResult OnPost(string author,[FromQuery] int? page = 1, int? pageNumber = null)
        {
            int currentPage = page ?? pageNumber ?? 1;
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized();

            if (!string.Equals(User.Identity!.Name, author, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            var trimmed = Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmed))
                ModelState.AddModelError(nameof(Text), "Cheep must not be empty.");
            else if (trimmed.Length > 160)
                ModelState.AddModelError(nameof(Text), "Cheep must be 1–160 characters.");

            if (!ModelState.IsValid)
            {
                Cheeps = _service.GetCheepsFromAuthor(author, currentPage, 32);
                ViewData["CurrentPage"] = currentPage;
                ViewData["Author"] = author;
                return Page();
            }

            _service.AddCheep(author, trimmed);
            
            return Redirect($"/{author}?page={currentPage}");
        }
    }
}