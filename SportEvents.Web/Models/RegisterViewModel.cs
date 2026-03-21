using System.ComponentModel.DataAnnotations;

namespace SportEvents.Web.Models
{
    public class RegisterViewModel
    {
        [Display(Name = "Имя")]
        [Required, StringLength(100)]
        public string FirstName { get; set; } = "";

        [Display(Name = "Фамилия")]
        [Required, StringLength(100)]
        public string LastName { get; set; } = "";

        [Display(Name = "Отчество")]
        [StringLength(100)]
        public string? MiddleName { get; set; }

        [Display(Name = "Дата рождения")]
        [Required, DataType(DataType.Date)]
        public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-18);

        [Display(Name = "Пол")]
        [Required]
        public bool Sex { get; set; }

        [Display(Name = "Телефон")]
        [Required]
        public string Phone { get; set; } = "+";

        [Display(Name = "Email")]
        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = "";

        [Display(Name = "Пароль")]
        [Required, DataType(DataType.Password), MinLength(8)]
        public string Password { get; set; } = "";

        [Display(Name = "Подтверждение пароля")]
        [Required, DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = "";

    }
}
