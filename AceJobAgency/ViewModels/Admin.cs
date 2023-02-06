using System.ComponentModel.DataAnnotations;

namespace AceJobAgency.ViewModels
{
    public class Admin
    {
        [Required, RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "First Name can only consist of letters")]
        [DataType(DataType.Text)]
        public string FirstName { get; set; }

        [Required, RegularExpression(@"^[A-Za-z]+$", ErrorMessage = "Last Name can only consist of letters")]
        [DataType(DataType.Text)]
        public string LastName { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Gender { get; set; }

        [Required, RegularExpression(@"^[STFG]\d{7}[A-Z]$", ErrorMessage = "Invaild NRIC")]
        [DataType(DataType.Text)]
        public string NRIC { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is requierd")]
        [MinLength(12, ErrorMessage = "Enter at least a 12 characters password")]
        [RegularExpression(@"^(?=.*\d)(?=.*[A-Z])(?=.*[a-z])(?=.*[^a-za-z0-9])(?!.*\s).{12,}",
            ErrorMessage = "Passwords must be at least 12 characters long and contain at least an upper case letter, lower case letter, digit and a symbol")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Password is requierd")]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }

        [Required]
        [DataType(DataType.Date)]
        [DateVaildation(18, ErrorMessage = " Minimum 18 year old")]
        public DateTime DateOfBirth { get; set; } = new DateTime(DateTime.Now.Year - 18, 1, 1);

       /* [Required(ErrorMessage = "Resume is requierd")]*/
        [Display(Name = "Resume")]
        public IFormFile? Resume { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string WhoamI { get; set; }



    }
}
