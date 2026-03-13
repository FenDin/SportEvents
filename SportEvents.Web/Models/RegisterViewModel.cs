using System.ComponentModel.DataAnnotations;

namespace MVC.Core.Sports_competitions.Models
{
    public class RegisterViewModel
    {
        [Required, StringLength(100)]
        public string FirstName { get; set; } = "";

        [Required, StringLength(100)]
        public string LastName { get; set; } = "";

        [StringLength(100)]
        public string? MiddleName { get; set; }

        [Required, DataType(DataType.Date)]
        public DateOnly BirthDate { get; set; } = DateOnly.FromDateTime(DateTime.Today.AddYears(-18));

        [Required]
        public bool Sex { get; set; }

        [Required]
        public string Phone { get; set; } = "+";

        [Required, EmailAddress, StringLength(255)]
        public string Email { get; set; } = "";

        [Required, DataType(DataType.Password), MinLength(8)]
        public string Password { get; set; } = "";

        [Required, DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; } = "";

    }
}
