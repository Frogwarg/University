using System.ComponentModel.DataAnnotations;

namespace University.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage ="E-mail обязателен")]
        [EmailAddress(ErrorMessage = "Этот E-mail адрес не действителен")]
        [Display(Name = "E-mail:")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Пароль обязателен")]
        [StringLength(100, ErrorMessage = "Пароль должен быть не менее {2} и не более {1} символов.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль:")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Повторите пароль:")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают.")]
        public string ConfirmPassword { get; set; }
    }
}
