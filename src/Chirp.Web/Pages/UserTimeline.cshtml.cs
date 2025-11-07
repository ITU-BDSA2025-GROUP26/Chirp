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

        public ActionResult OnGet(string author, [FromQuery] int page = 1)
        {
            int pageSize = 32;
            Cheeps = _service.GetCheepsFromAuthor(author, page, pageSize);
            return Page();
        }
        public IActionResult OnPost(string author, int page = 1)
        {
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
                Cheeps = _service.GetCheepsFromAuthor(author,page, 32);
                return Page();
            }

            _service.AddCheep(author, trimmed);
            return RedirectToPage(null, new { author, page });
        }
    }
}