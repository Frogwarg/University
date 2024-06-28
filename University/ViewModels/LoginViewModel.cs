using System.ComponentModel.DataAnnotations;

namespace University.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "E-mail обязателен")]
        [EmailAddress(ErrorMessage = "Этот E-mail адрес не действителен")]
        [Display(Name ="E-mail:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль:")]
        public string Password { get; set; }

        [Display(Name = "Запомнить меня?")]
        public bool RememberMe { get; set; }
    }
}
