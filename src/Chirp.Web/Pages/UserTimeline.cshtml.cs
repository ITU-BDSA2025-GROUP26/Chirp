using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Infrastructure.Chirp.Service;
using Chirp.Core;
using Chirp.Razor.Areas.Identity.Data;

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

            if ()
            {
                Cheeps = _service.GetCheepsFromAuthor(author, page, pageSize);
            }
            else
            {
                for (String followed; )
                {
                    Cheeps.Add(_service.GetCheepsFromAuthor(followed, page, pagesize));
                }
            }
            return Page();
        }
    }
}