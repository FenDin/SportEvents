using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SportEvents.Web.Models.Schools;

public sealed class SchoolCatalogViewModel
{
    public string? SearchQuery { get; init; }
    public bool CanManage { get; init; }
    public IReadOnlyList<SchoolListItemViewModel> Items { get; init; } = Array.Empty<SchoolListItemViewModel>();
}

public sealed class SchoolListItemViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int ParticipantCount { get; init; }
    public IReadOnlyList<string> SportSubTypes { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> ParticipantNames { get; init; } = Array.Empty<string>();
}

public sealed class SchoolDetailsViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public int ParticipantCount { get; init; }
    public bool CanManage { get; init; }
    public IReadOnlyList<string> SportSubTypes { get; init; } = Array.Empty<string>();
    public IReadOnlyList<SchoolParticipantViewModel> Participants { get; init; } = Array.Empty<SchoolParticipantViewModel>();
}

public sealed class SchoolParticipantViewModel
{
    public string FullName { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public string PhotoUrl { get; init; } = string.Empty;
}

public sealed class SchoolEditViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Название школы")]
    [Required(ErrorMessage = "Введите название школы.")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Описание")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Display(Name = "Дисциплины")]
    public List<int> SelectedSportSubtypeIds { get; set; } = new();

    [Display(Name = "Учащиеся и выпускники")]
    public List<int> SelectedParticipantIds { get; set; } = new();

    public IReadOnlyList<SelectListItem> SportSubtypeOptions { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> ParticipantOptions { get; set; } = Array.Empty<SelectListItem>();
}
