using AceJobAgency.GoogleRecaptcha;
using AceJobAgency.Model;
using AceJobAgency.ViewModels;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AceJobAgency.Pages
{
	public class LoginModel : PageModel
	{
		private readonly GoogleCaptchaService _GoogleCaptchaService;
		[BindProperty]
		public Login LModel { get; set; }

		private readonly SignInManager<ApplicationUser> signInManager;

		private readonly IHttpContextAccessor contxt;

		public LoginModel(SignInManager<ApplicationUser> signInManager, IHttpContextAccessor httpContextAccessor, GoogleCaptchaService googleCaptchaService)
		{
			this.signInManager = signInManager;
			_GoogleCaptchaService = googleCaptchaService;
			contxt = httpContextAccessor;
		}
		public void OnGet()
		{
		
		}


		public async Task<IActionResult> OnPostAsync()
		{

			var _GoogleCaptcha = _GoogleCaptchaService.ResVer(LModel.Token);
			if (!_GoogleCaptcha.Result.success && _GoogleCaptcha.Result.score >= 0.5)
			{
				ModelState.AddModelError("", "You are not human");

			}
			if (ModelState.IsValid)
			{
			
				var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password, LModel.RememberMe, lockoutOnFailure: true);

				if (identityResult.IsLockedOut)
				{
					ModelState.AddModelError("LockoutError", "The account is locked out");
					return Page();
				}
				if (identityResult.Succeeded)
				{
					var claims = new List<Claim> {
					new Claim(ClaimTypes.Name, "c@c.com"),
					new Claim(ClaimTypes.Email, "c@c.com"),
			
					};
					var i = new ClaimsIdentity(claims, "MyCookieAuth");
					ClaimsPrincipal claimsPrincipal = new ClaimsPrincipal(i);
					await HttpContext.SignInAsync("MyCookieAuth", claimsPrincipal);

					//session

					string session = Guid.NewGuid().ToString();
					HttpContext.Session.SetString("AuthSession", session);
					Response.Cookies.Append("AuthSession", session);



					/*	contxt.HttpContext.Session.SetString("Membership", LModel.Email);
	*/
					return RedirectToPage("/Index");

				}
				ModelState.AddModelError("", "Username or Password incorrect");
			}
			return Page();
		}

	}
}