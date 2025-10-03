using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Razor;

namespace Chirp.Razor.Pages
{

    public class PublicModel : PageModel
    {
        private readonly ICheepService _service;
        public List<CheepViewModel> Cheeps { get; set; } = new();

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
    }
}