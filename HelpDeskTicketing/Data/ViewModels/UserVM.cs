using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace HelpDeskTicketing.Data.ViewModels
{
    public class UserVM
    {

        public string Id { get; set; }

        [Required]
        [Display(Name = "Full name")]
        [RegularExpression(@"^[A-Z][a-z]{2,}([ ][A-Z][a-z]{2,})+$", ErrorMessage = "Enter a valid name")]
        public string FullName { get; set; }

        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        [Display(Name ="Phone number")]
        [RegularExpression(@"^07[0-9]{8}$")]
        public string PhoneNumber { get; set; }

        [Required]
        [Display(Name = "Branch")]
        public string BranchId { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match!")]
        public string ConfirmPassword { get; set; }

    }
}
