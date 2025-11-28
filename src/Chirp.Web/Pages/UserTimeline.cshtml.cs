using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Core;
using Chirp.Core.Models;

namespace Chirp.Web.Pages
{
    public class UserTimelineModel : PageModel
    {
        private readonly ICheepService _service;
        private readonly IAuthorService _authorService;
        public List<CheepDto> Cheeps { get; set; } = new();

        public List<Author> Following { get; set; } = new();

        [BindProperty]
        public string Text { get; set; } = string.Empty;
        public UserTimelineModel(ICheepService service, IAuthorService authorService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _authorService = authorService ?? throw new ArgumentNullException(nameof(authorService));
        }

        public ActionResult OnGet(string author, [FromQuery] int? page = 1, int? pageNumber = null)
        {
            if (_service is null)
                throw new InvalidOperationException("PublicModel: _service is null in OnGet (unit test");
            
            var safeAuthor = author ?? string.Empty;
            
            int currentPage = page ?? pageNumber ?? 1;
            const int pageSize = 32;
            var loggedInUser = User?.Identity?.Name;

            // If viewing own timeline while logged in: show self + followees
            if (!string.IsNullOrEmpty(loggedInUser) &&
                string.Equals(loggedInUser, safeAuthor, StringComparison.OrdinalIgnoreCase))
            {
                var allCheeps = new List<CheepDto>();

                // own cheeps
                allCheeps.AddRange(_service.GetCheepsFromAuthor(safeAuthor, currentPage, pageSize));

                // followees
                var followees = _authorService.GetFollowing(safeAuthor).Result;
                foreach (var f in followees)
                {
                    if (!string.IsNullOrEmpty(f.UserName))
                    {
                        allCheeps.AddRange(_service.GetCheepsFromAuthor(f.UserName, currentPage, pageSize));
                    }
                }

                // sort by timestamp descending (assuming parsable format)
                Cheeps = allCheeps
                    .OrderByDescending(c => DateTime.Parse(c.TimeStamp))
                    .ToList();

                Following = followees;
            }
            else
            {
                // Viewing someone else's timeline: only that author's cheeps
                Cheeps = _service.GetCheepsFromAuthor(safeAuthor, currentPage, pageSize);

                if (!string.IsNullOrEmpty(loggedInUser))
                    Following = _authorService.GetFollowing(loggedInUser).Result;
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
            
            return RedirectToPage("/UserTimeline", new { author = author, page = currentPage });
        }
        
        public IActionResult OnPostFollow(string authorToFollow, string author, [FromQuery] int? page = 1, int? pageNumber = null)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized();

            var follower = User.Identity!.Name!;
            _authorService.Follow(follower, authorToFollow).Wait();

            int currentPage = page ?? pageNumber ?? 1;
            return RedirectToPage("/UserTimeline", new { author = author, page = currentPage });
        }
        
        public IActionResult OnPostUnfollow(string authorToUnfollow, string author, [FromQuery] int? page = 1, int? pageNumber = null)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized();

            var follower = User.Identity!.Name!;
            _authorService.Unfollow(follower, authorToUnfollow).Wait();

            int currentPage = page ?? pageNumber ?? 1;
            return RedirectToPage("/UserTimeline", new { author = author, page = currentPage });
        }
    }
}