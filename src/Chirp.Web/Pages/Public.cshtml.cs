using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Core;
using System.ComponentModel.DataAnnotations;
using Chirp.Core.Models;

namespace Chirp.Web.Pages
{

    public class PublicModel : PageModel
    {
        private readonly ICheepService _service;
        private readonly IAuthorService _authorService;
        public List<CheepDto> Cheeps { get; set; } = new();
        
        public List<Author> Following { get; set; } = new();

        [BindProperty]
        [Display(Name = "What's on your mind?")]
        [StringLength(160, ErrorMessage = "Cheep must be at most 160 characters.")]
        public string Text { get; set; } = string.Empty;
        public PublicModel(ICheepService service, IAuthorService authorService)
        {
            _service = service ?? throw new ArgumentNullException(nameof(service));
            _authorService = authorService ?? throw new ArgumentNullException(nameof(authorService));
        }

        public ActionResult OnGet([FromQuery] int? page = 1, int? pageNumber = null)
        {
            if (_service is null)
                throw new InvalidOperationException("PublicModel: _service is null in OnGet (unit test");
            
            int currentPage = page ?? pageNumber ?? 1;
            const int pageSize = 32;
            
            Cheeps = _service.GetCheeps(currentPage, pageSize);

            if (User?.Identity?.IsAuthenticated ?? false)
            {
                var userName = User.Identity!.Name!;
                Following = _authorService.GetFollowing(userName).Result;
            }
            
            ViewData["CurrentPage"] = currentPage;
            return Page();
        }

        public IActionResult OnPost([FromQuery] int? page = 1, int? pageNumber = null)
        {
            int currentPage = page ?? pageNumber ?? 1;
        
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
                Cheeps = _service.GetCheeps(currentPage, 32);
                
                if (User?.Identity?.IsAuthenticated ?? false)
                {
                    var userName = User.Identity!.Name!;
                    Following = _authorService.GetFollowing(userName).Result;
                }
                
                ViewData["CurrentPage"] = currentPage;
                return Page();
            }

            _service.AddCheep(User.Identity!.Name!, trimmed);

            // Post‑Redirect‑Get pattern prevents duplicate submissions on refresh
            return Redirect($"?page={currentPage}");
        }
        
        public IActionResult OnPostFollow(string authorToFollow, [FromQuery] int? page = 1, int? pageNumber = null)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized();

            var follower = User.Identity!.Name!;
            _authorService.Follow(follower, authorToFollow).Wait();

            int currentPage = page ?? pageNumber ?? 1;
            return Redirect($"?page={currentPage}");
        }
        
        public IActionResult OnPostUnfollow(string authorToUnfollow, [FromQuery] int? page = 1, int? pageNumber = null)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized();

            var follower = User.Identity!.Name!;
            _authorService.Unfollow(follower, authorToUnfollow).Wait();

            int currentPage = page ?? pageNumber ?? 1;
            return Redirect($"?page={currentPage}");
        }
        
        public IActionResult OnPostLike(int cheepId, [FromQuery] int? page = 1, int? pageNumber = null)
        {
            if (!(User?.Identity?.IsAuthenticated ?? false))
                return Unauthorized();

            int currentPage = page ?? pageNumber ?? 1;

            var userName = User.Identity!.Name!;
            _service.LikeCheep(userName, cheepId);

            return Redirect($"?page={currentPage}");
        }
    }
}