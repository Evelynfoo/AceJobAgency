using AceJobAgency.Model;
using AceJobAgency.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AceJobAgency.Pages
{
    [Authorize(Roles = "Admin")]
    public class AdminModel : PageModel
    {
        private UserManager<ApplicationUser> userManager { get; }

        private SignInManager<ApplicationUser> signInManager { get; }

        private readonly RoleManager<IdentityRole> roleManager;

        private IWebHostEnvironment _uploadFile;




        [BindProperty]
        public Admin AModel { get; set; }

        private readonly IHttpContextAccessor contxt;

        public AdminModel(UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager, IWebHostEnvironment uploadFile,
        IHttpContextAccessor httpContextAccessor, RoleManager<IdentityRole> roleManager)
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

                if (AModel.Resume != null)
                {
                    if (AModel.Resume.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Upload", "File size cannot exceed 2MB.");
                        return Page();
                    }

                    var uploads = "uploads";
                    var imageFile = Guid.NewGuid() + Path.GetExtension(AModel.Resume.FileName);
                    var imagePath = Path.Combine(_uploadFile.ContentRootPath, "wwwroot", uploads, imageFile);
                    using var fileStream = new FileStream(imagePath, FileMode.Create);
                    await AModel.Resume.CopyToAsync(fileStream);
                    myResumes = string.Format("/{0}/{1}", uploads, imageFile);
                }

                var admin = new ApplicationUser()
                {
                    UserName = AModel.Email,
                    Email = AModel.Email,
                    FirstName = AModel.FirstName,
                    LastName = AModel.LastName,
                    Gender = AModel.Gender,
                    NRIC = protector.Protect(AModel.NRIC),
                    DateOfBirth = AModel.DateOfBirth,
                    WhoAmI = AModel.WhoamI,
                    Resume = myResumes
                };
                IdentityRole role = await roleManager.FindByIdAsync("Admin");
                if (role == null)
                {
                    IdentityResult results2 = await roleManager.CreateAsync(new IdentityRole("Admin"));
                    if (!results2.Succeeded)
                    {
                        ModelState.AddModelError("", "Create role Admin failed");
                    }
                }

                var result = await userManager.CreateAsync(admin, AModel.Password);
                if (result.Succeeded)
                {

                    result = await userManager.AddToRoleAsync(admin, "Admin");
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
