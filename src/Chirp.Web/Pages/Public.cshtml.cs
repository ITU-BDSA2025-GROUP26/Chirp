using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Core;
using System.ComponentModel.DataAnnotations;

namespace Chirp.Razor.Pages
{

    public class PublicModel : PageModel
    {
        private readonly ICheepService _service;
        public List<CheepDto> Cheeps { get; set; } = new();

        [BindProperty]
        [Display(Name = "What's on your mind?")]
        [StringLength(160, ErrorMessage = "Cheep must be at most 160 characters.")]
        public string Text { get; set; } = string.Empty;
        public PublicModel(ICheepService service)
        {
            _service = service;
        }

        public ActionResult OnGet([FromQuery] int page = 1)
        {
            int pageSize = 32;
            Cheeps = _service.GetCheeps(page, pageSize);
            return Page();
        }

        public IActionResult OnPost([FromQuery] int pageNumber = 1)
        {
            // Only authenticated users may post
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized();

            // Manual validation in addition to data annotations
            var trimmed = Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(trimmed))
                ModelState.AddModelError(nameof(Text), "Cheep must not be empty.");
            else if (trimmed.Length > 160)
                ModelState.AddModelError(nameof(Text), "Cheep must be 1–160 characters.");

            if (!ModelState.IsValid)
            {
                Cheeps = _service.GetCheeps(pageNumber, 32);
                return Page();
            }

            _service.AddCheep(User.Identity!.Name!, trimmed);

            // Post‑Redirect‑Get pattern prevents duplicate submissions on refresh
            return RedirectToPage("/Public",new { pageNumber });
        }
    }
}