using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Core;
using Chirp.Core.Models;
using Chirp.Razor.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace Chirp.Web.Pages
{
    public class UserTimelineModel : PageModel
    {
        private readonly UserManager<Author> _userManager;
        private readonly ICheepService _service;

        private Author? _currentUser;
        public List<CheepDto> Cheeps { get; set; } = new();

        public UserTimelineModel(ICheepService service,
            UserManager<Author> userManager)
        {
            _service = service;
            _userManager = userManager;
            _currentUser = _userManager?.GetUserAsync(HttpContext.User).Result;
        }

        public ActionResult OnGet(string author, [FromQuery] int page = 1)
        {
            int pageSize = 32;
            
            if (_currentUser != null && _currentUser.UserName == author)
            {
                foreach(var followed in _currentUser.FollowedUsers)
                {
                    Cheeps.AddRange(_service.GetCheepsFromAuthor(followed, page, pageSize));
                }
            }
            else
            {
                Cheeps = _service.GetCheepsFromAuthor(author, page, pageSize);
            }
            return Page();
        }
    }
}