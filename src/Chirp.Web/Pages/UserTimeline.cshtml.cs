using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Core;

namespace Chirp.Web.Pages
{
    public class UserTimelineModel : PageModel
    {
        private readonly ICheepService _service;
        public List<CheepDto> Cheeps { get; set; } = new();

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
    }
}