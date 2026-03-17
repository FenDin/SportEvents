using System.ComponentModel.DataAnnotations;

namespace SportEvents.Web.Models
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
        public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-18);

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
