using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SportEvents.Web.Models.Admin;

public sealed class AdminUsersViewModel
{
    public string? StatusMessage { get; init; }
    public string StatusType { get; init; } = "info";
    public IReadOnlyList<RoleManagementViewModel> Roles { get; init; } = Array.Empty<RoleManagementViewModel>();
    public RoleCreateViewModel CreateRole { get; init; } = new();
    public IReadOnlyList<UserRoleViewModel> Users { get; init; } = Array.Empty<UserRoleViewModel>();
}

public sealed class RoleManagementViewModel
{
    public int RoleId { get; init; }
    public string Title { get; init; } = string.Empty;
    public int UserCount { get; init; }
    public bool IsSystemRole { get; init; }
}

public sealed class RoleCreateViewModel
{
    [Display(Name = "Название роли")]
    [Required(ErrorMessage = "Введите название роли.")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;
}

public sealed class UserRoleViewModel
{
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string PhotoUrl { get; init; } = string.Empty;
    public string RoleTitle { get; init; } = string.Empty;
    public int SelectedRoleId { get; init; }
    public bool IsCurrentUser { get; init; }
    public IReadOnlyList<SelectListItem> AvailableRoles { get; init; } = Array.Empty<SelectListItem>();
}

public sealed class AdminUserEditViewModel
{
    public int UserId { get; set; }

    [Display(Name = "Имя")]
    [Required(ErrorMessage = "Введите имя.")]
    [StringLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Display(Name = "Фамилия")]
    [Required(ErrorMessage = "Введите фамилию.")]
    [StringLength(100)]
    public string LastName { get; set; } = string.Empty;

    [Display(Name = "Отчество")]
    [StringLength(100)]
    public string? MiddleName { get; set; }

    [Display(Name = "Дата рождения")]
    [Required(ErrorMessage = "Укажите дату рождения.")]
    [DataType(DataType.Date)]
    public DateTime BirthDate { get; set; } = DateTime.Today.AddYears(-18);

    [Display(Name = "Пол")]
    public bool Sex { get; set; }

    [Display(Name = "Телефон")]
    [Required(ErrorMessage = "Введите телефон.")]
    [StringLength(32)]
    public string Phone { get; set; } = "+";

    [Display(Name = "Email")]
    [Required(ErrorMessage = "Введите email.")]
    [EmailAddress(ErrorMessage = "Укажите корректный email.")]
    [StringLength(255)]
    public string Email { get; set; } = string.Empty;

    [Display(Name = "Ссылка на фото")]
    [StringLength(512)]
    public string? PhotoUrl { get; set; }

    [Display(Name = "Роль")]
    [Required(ErrorMessage = "Выберите роль.")]
    public int? RoleId { get; set; }

    [Display(Name = "Новый пароль")]
    [DataType(DataType.Password)]
    public string? NewPassword { get; set; }

    [Display(Name = "Подтверждение пароля")]
    [DataType(DataType.Password)]
    [Compare(nameof(NewPassword), ErrorMessage = "Пароли не совпадают.")]
    public string? ConfirmPassword { get; set; }

    public IReadOnlyList<SelectListItem> AvailableRoles { get; set; } = Array.Empty<SelectListItem>();
}
