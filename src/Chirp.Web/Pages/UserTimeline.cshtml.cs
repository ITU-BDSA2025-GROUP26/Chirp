using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Core;

namespace Chirp.Razor.Pages
{
    public class UserTimelineModel : PageModel
    {
        private readonly ICheepService _service;
        public List<CheepDto> Cheeps { get; set; } = new();

        [BindProperty]
        public string Text { get; set; }
        public UserTimelineModel(ICheepService service)
        {
            _service = service;
        }

        public ActionResult OnGet(string author, [FromQuery] int? page = 1, int? pageNumber = null)
        {
            int currentPage = page ?? pageNumber ?? 1;
            const int pageSize = 32;
            Cheeps = _service.GetCheepsFromAuthor(author, currentPage, pageSize);
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