using System.ComponentModel.DataAnnotations;

namespace University.ViewModels
{
    public class ForgotPasswordViewModel
    {
        [Required(ErrorMessage = "E-mail обязателен")]
        [EmailAddress(ErrorMessage = "Этот E-mail адрес не действителен")]
        [Display(Name = "E-mail:")]
        public string Email { get; set; }
    }
}
