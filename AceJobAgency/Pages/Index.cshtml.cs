using AceJobAgency.Model;
using AceJobAgency.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;


namespace AceJobAgency.Pages
{
    [Authorize]
	public class IndexModel : PageModel
    {
		 private readonly ILogger<IndexModel> _logger;

		private readonly IHttpContextAccessor contxt;

		private UserManager<ApplicationUser> UserManager { get; }
        private AuthDbContext authDbContext;

        [BindProperty]
        public ApplicationUser user { get; set; }

        [BindProperty]
		private SignInManager<ApplicationUser> signInManager { get; }


		public IndexModel(ILogger<IndexModel> logger, UserManager<ApplicationUser> userManager,
		SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, AuthDbContext authDbContext)
        {
            _logger = logger;
			UserManager = userManager;
			this.signInManager = signInManager;
			contxt = httpContextAccessor;
			this.authDbContext = authDbContext;

		}
		public const string SessionName = "Name";
		public const string SessionEmail = "Email";

		public IActionResult OnGet()
        {
			//session

			string session = Request.Cookies["AuthSession"];
			if (string.IsNullOrEmpty(HttpContext.Session.GetString("AuthSession")))
			{
				HttpContext.Session.Remove("AuthSession");
				signInManager.SignOutAsync();
				HttpContext.Response.Redirect("Login");
			}

			if (string.IsNullOrEmpty(HttpContext.Session.GetString(SessionName)))
			{
				HttpContext.Session.SetString(SessionName, "AceJobAgency");
				HttpContext.Session.SetString(SessionEmail, "AceJobAgency@gmail.com");

			}
			var name = HttpContext.Session.GetString(SessionName);
			var email = HttpContext.Session.GetString(SessionEmail);

			_logger.LogInformation("Session Name: {Name}", name);
			_logger.LogInformation("Session Email: {Email}", email);

			// retieve data 

			var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
			var protector = dataProtectionProvider.CreateProtector("MySecretKey");

			string userId = UserManager.GetUserId(User);
			ApplicationUser? currentUser = authDbContext.Users.FirstOrDefault(x => x.Id.Equals(userId));
			user = currentUser;
			user.NRIC = protector.Unprotect(currentUser.NRIC);

			

			//audit log
			_logger.LogInformation("About page visited at {DT}",DateTime.UtcNow.ToLongTimeString());

			
            return Page();
        }
     
    }
}