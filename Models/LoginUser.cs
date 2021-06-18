using System.ComponentModel.DataAnnotations;

namespace LoginAndRegistration.Models
{
  
    public class LoginUser
    {
        [Display(Name = "Email Address:")]
        [Required(ErrorMessage = "is required.")]
        [EmailAddress]
        [DataType(DataType.EmailAddress)]
        public string EmailLogin { get; set; }

        [Required(ErrorMessage = "is required.")]
        [DataType(DataType.Password)]
        public string PasswordLogin { get; set; }
    }
}