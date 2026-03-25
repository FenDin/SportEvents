using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SportEvents.Web.Models.Competitions;

public sealed class CompetitionCatalogViewModel
{
    public string? SearchQuery { get; init; }
    public string Status { get; init; } = "all";
    public bool CanManage { get; init; }
    public IReadOnlyList<CompetitionListItemViewModel> Items { get; init; } = Array.Empty<CompetitionListItemViewModel>();
}

public sealed class CompetitionListItemViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string PhotoUrl { get; init; } = string.Empty;
    public DateTime? DateStart { get; init; }
    public DateTime? DateEnd { get; init; }
    public string SportTitle { get; init; } = string.Empty;
    public string SportTypeTitle { get; init; } = string.Empty;
    public string SportSubTypeTitle { get; init; } = string.Empty;
    public int ParticipantCount { get; init; }
    public IReadOnlyList<string> Events { get; init; } = Array.Empty<string>();
}

public sealed class CompetitionDetailsViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string PhotoUrl { get; init; } = string.Empty;
    public DateTime? DateStart { get; init; }
    public DateTime? DateEnd { get; init; }
    public string SportTitle { get; init; } = string.Empty;
    public string SportTypeTitle { get; init; } = string.Empty;
    public string SportSubTypeTitle { get; init; } = string.Empty;
    public int ParticipantCount { get; init; }
    public bool CanManage { get; init; }
    public bool CanEnroll { get; init; }
    public bool IsUserEnrolled { get; init; }
    public IReadOnlyList<CompetitionEventLinkViewModel> Events { get; init; } = Array.Empty<CompetitionEventLinkViewModel>();
    public IReadOnlyList<CompetitionParticipantViewModel> Participants { get; init; } = Array.Empty<CompetitionParticipantViewModel>();
}

public sealed class CompetitionEventLinkViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string PhotoUrl { get; init; } = string.Empty;
    public DateTime? DateStart { get; init; }
    public DateTime? DateEnd { get; init; }
}

public sealed class CompetitionParticipantViewModel
{
    public int Id { get; init; }
    public string FullName { get; init; } = string.Empty;
    public string PhotoUrl { get; init; } = string.Empty;
    public string? SchoolTitle { get; init; }
    public string Email { get; init; } = string.Empty;
    public string? Phone { get; init; }
    public bool IsCurrentUser { get; init; }
}

public sealed class CompetitionEditViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Название соревнования")]
    [Required(ErrorMessage = "Введите название соревнования.")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    [Display(Name = "Описание")]
    [StringLength(255)]
    public string? Description { get; set; }

    [Display(Name = "Дата начала")]
    [Required(ErrorMessage = "Укажите дату начала.")]
    [DataType(DataType.DateTime)]
    public DateTime? DateStart { get; set; }

    [Display(Name = "Дата окончания")]
    [Required(ErrorMessage = "Укажите дату окончания.")]
    [DataType(DataType.DateTime)]
    public DateTime? DateEnd { get; set; }

    [Display(Name = "Событие")]
    [Required(ErrorMessage = "Выберите событие.")]
    public int? EventId { get; set; }

    [Display(Name = "Дисциплина")]
    [Required(ErrorMessage = "Выберите дисциплину.")]
    public int? SportSubTypeId { get; set; }

    [Display(Name = "Ссылка на фото")]
    [StringLength(512)]
    public string? PhotoUrl { get; set; }

    public IReadOnlyList<SelectListItem> EventOptions { get; set; } = Array.Empty<SelectListItem>();
    public IReadOnlyList<SelectListItem> SportSubtypeOptions { get; set; } = Array.Empty<SelectListItem>();
}
