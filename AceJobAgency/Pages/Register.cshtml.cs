using AceJobAgency.Model;
using AceJobAgency.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting;
using System;

namespace AceJobAgency.Pages
{
    public class RegisterModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }

        private SignInManager<ApplicationUser> signInManager { get; }

        private readonly RoleManager<IdentityRole> roleManager;

        private IWebHostEnvironment _uploadFile;

		[BindProperty]
        public Register RModel { get; set; }

		private readonly IHttpContextAccessor contxt;

		public RegisterModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, IWebHostEnvironment uploadFile, 
        IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole>roleManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
			_uploadFile = uploadFile;
            contxt = httpContextAccessor;
            this.roleManager = roleManager;
     
        }
        public void OnGet()
        {
        }
        //Save data into the database
        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
			{
                var myResumes = string.Empty;
                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");
                
                if (RModel.Resume != null)
                {
                    if (RModel.Resume.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Upload", "File size cannot exceed 2MB.");
                        return Page();
                    }

                    var uploads = "uploads";
                    var imageFile = Guid.NewGuid() + Path.GetExtension(RModel.Resume.FileName);
                    var imagePath = Path.Combine(_uploadFile.ContentRootPath, "wwwroot", uploads, imageFile);
                    using var fileStream = new FileStream(imagePath, FileMode.Create);
                    await RModel.Resume.CopyToAsync(fileStream);
                    myResumes = string.Format("/{0}/{1}", uploads, imageFile);
                }

                var user = new ApplicationUser()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    FirstName = RModel.FirstName,
                    LastName = RModel.LastName,
                    Gender = RModel.Gender,
                    NRIC = protector.Protect(RModel.NRIC),
                    DateOfBirth = RModel.DateOfBirth,
                    WhoAmI = RModel.WhoamI,
                    Resume = myResumes
                };
                IdentityRole role = await roleManager.FindByIdAsync("Customer");
                if (role == null)
                {
                    IdentityResult results2 = await roleManager.CreateAsync(new IdentityRole("Customer"));
                    if (!results2.Succeeded)
                    {
                        ModelState.AddModelError("", "Create role customer failed");
                    }
                }

                var result = await userManager.CreateAsync(user, RModel.Password);
                if (result.Succeeded)
                {

                    result = await userManager.AddToRoleAsync(user, "Customer");
                    /*await signInManager.SignInAsync(user, false);*/
					return RedirectToPage("Login");


				}
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }

             

			}
				
            return Page();
        }
    }
}
