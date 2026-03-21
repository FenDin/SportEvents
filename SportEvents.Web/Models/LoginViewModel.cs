using System.ComponentModel.DataAnnotations;

namespace SportEvents.Web.Models
{
    public class LoginViewModel
    {
        [Display(Name = "Email")]
        [Required, EmailAddress]
        public string Email { get; set; } = "";

        [Display(Name = "Пароль")]
        [Required, DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [Display(Name = "Запомнить меня")]
        public bool RememberMe { get; set; }
    }
}
