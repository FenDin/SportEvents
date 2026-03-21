using System.ComponentModel.DataAnnotations;

namespace SportEvents.Web.Models.Events;

public sealed class EventCatalogViewModel
{
    public string? SearchQuery { get; init; }
    public string Status { get; init; } = "all";
    public bool CanManage { get; init; }
    public IReadOnlyList<EventListItemViewModel> Items { get; init; } = Array.Empty<EventListItemViewModel>();
}

public sealed class EventListItemViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? DateStart { get; init; }
    public DateTime? DateEnd { get; init; }
    public int CompetitionCount { get; init; }
    public IReadOnlyList<string> Sports { get; init; } = Array.Empty<string>();
}

public sealed class EventDetailsViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? DateStart { get; init; }
    public DateTime? DateEnd { get; init; }
    public bool CanManage { get; init; }
    public IReadOnlyList<EventCompetitionViewModel> Competitions { get; init; } = Array.Empty<EventCompetitionViewModel>();
}

public sealed class EventCompetitionViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public DateTime? DateStart { get; init; }
    public DateTime? DateEnd { get; init; }
    public string SportTitle { get; init; } = string.Empty;
    public string SportTypeTitle { get; init; } = string.Empty;
    public string SportSubTypeTitle { get; init; } = string.Empty;
}

public sealed class EventEditViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Название события")]
    [Required(ErrorMessage = "Введите название события.")]
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
}
