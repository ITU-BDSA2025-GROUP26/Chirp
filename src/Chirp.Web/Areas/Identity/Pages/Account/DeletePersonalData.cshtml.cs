using System.Linq;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Chirp.Core.Models;
using Chirp.Infrastructure;

namespace Chirp.Razor.Areas.Identity.Pages.Account
{
    public class DeletePersonalDataModel : PageModel
    {
        private readonly UserManager<Author> _userManager;
        private readonly SignInManager<Author> _signInManager;
        private readonly ChirpDBContext _context;

        public DeletePersonalDataModel(
            UserManager<Author> userManager,
            SignInManager<Author> signInManager,
            ChirpDBContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        public async Task<IActionResult> OnGet()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            
            var userCheeps = _context.Cheeps.Where(c => c.AuthorId == user.Id);
            _context.Cheeps.RemoveRange(userCheeps);
            await _context.SaveChangesAsync();
            
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                throw new InvalidOperationException($"Unexpected error occurred deleting user.");
            }
            
            await _signInManager.SignOutAsync();
            return Redirect("~/");
        }
    }
}