using System.ComponentModel.DataAnnotations;

namespace HelpDeskTicketing.Data.ViewModels
{
    public class UserLoginVM
    {

        [Required]
        [Display(Name ="User Name")]
        public string UserName { get; set; }

        [Required]
        public string Password { get; set; }

    }
}
