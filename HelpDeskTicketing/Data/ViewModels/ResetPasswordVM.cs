using System.ComponentModel.DataAnnotations;

namespace HelpDeskTicketing.Data.ViewModels
{
    public class ResetPasswordVM
    {

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm Password")]
        [Compare("Password",ErrorMessage ="The passwords do not match!")]
        public string ConfirmPassword { get; set; }

    }
}
