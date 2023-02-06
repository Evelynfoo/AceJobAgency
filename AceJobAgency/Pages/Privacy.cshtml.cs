using AceJobAgency.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages
{
	[Authorize]
	public class PrivacyModel : PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
		private SignInManager<ApplicationUser> signInManager { get; }
		public PrivacyModel(ILogger<PrivacyModel> logger, SignInManager<ApplicationUser> signInManager)
        {
            _logger = logger;
			this.signInManager = signInManager;
		}

        public IActionResult OnGet()
        {
			/*if (string.IsNullOrEmpty(HttpContext.Session.GetString("AuthSession")))
			{
				HttpContext.Session.Remove("AuthSession");
				signInManager.SignOutAsync();
				HttpContext.Response.Redirect("Login");
			}*/

			return Page();
		}
    }
}