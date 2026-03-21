using Microsoft.AspNetCore.Mvc.Rendering;

namespace SportEvents.Web.Models.Admin;

public sealed class AdminUsersViewModel
{
    public IReadOnlyList<UserRoleViewModel> Users { get; init; } = Array.Empty<UserRoleViewModel>();
}

public sealed class UserRoleViewModel
{
    public int UserId { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string RoleTitle { get; init; } = string.Empty;
    public int SelectedRoleId { get; init; }
    public IReadOnlyList<SelectListItem> AvailableRoles { get; init; } = Array.Empty<SelectListItem>();
}
