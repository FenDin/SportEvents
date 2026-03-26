using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace SportEvents.Web.Models.Sports;

public sealed class SportCatalogViewModel
{
    public string? SearchQuery { get; init; }
    public bool CanManage { get; init; }
    public IReadOnlyList<SportListItemViewModel> Items { get; init; } = Array.Empty<SportListItemViewModel>();
}

public sealed class SportListItemViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int SportTypeCount { get; init; }
    public int SportSubtypeCount { get; init; }
    public IReadOnlyList<string> SportTypeTitles { get; init; } = Array.Empty<string>();
}

public sealed class SportDetailsViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int SportTypeCount { get; init; }
    public int SportSubtypeCount { get; init; }
    public bool CanManage { get; init; }
    public bool CanDelete { get; init; }
    public string? DeleteBlockedReason { get; init; }
    public IReadOnlyList<SportTypeSummaryViewModel> SportTypes { get; init; } = Array.Empty<SportTypeSummaryViewModel>();
}

public sealed class SportTypeSummaryViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int SportSubtypeCount { get; init; }
    public IReadOnlyList<string> SportSubtypeTitles { get; init; } = Array.Empty<string>();
}

public sealed class SportEditViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Вид спорта")]
    [Required(ErrorMessage = "Введите название вида спорта.")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;
}

public sealed class SportTypeCatalogViewModel
{
    public string? SearchQuery { get; init; }
    public bool CanManage { get; init; }
    public IReadOnlyList<SportTypeListItemViewModel> Items { get; init; } = Array.Empty<SportTypeListItemViewModel>();
}

public sealed class SportTypeListItemViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int SportId { get; init; }
    public string SportTitle { get; init; } = string.Empty;
    public int SportSubtypeCount { get; init; }
    public IReadOnlyList<string> SportSubtypeTitles { get; init; } = Array.Empty<string>();
}

public sealed class SportTypeDetailsViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int SportId { get; init; }
    public string SportTitle { get; init; } = string.Empty;
    public int SportSubtypeCount { get; init; }
    public bool CanManage { get; init; }
    public bool CanDelete { get; init; }
    public string? DeleteBlockedReason { get; init; }
    public IReadOnlyList<SportSubtypeSummaryViewModel> SportSubtypes { get; init; } = Array.Empty<SportSubtypeSummaryViewModel>();
}

public sealed class SportSubtypeSummaryViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int CompetitionCount { get; init; }
    public int SchoolCount { get; init; }
}

public sealed class SportTypeEditViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Вид спорта")]
    [Required(ErrorMessage = "Выберите вид спорта.")]
    public int? SportId { get; set; }

    [Display(Name = "Тип спорта")]
    [Required(ErrorMessage = "Введите название типа спорта.")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    public IReadOnlyList<SelectListItem> SportOptions { get; set; } = Array.Empty<SelectListItem>();
}

public sealed class SportSubtypeCatalogViewModel
{
    public string? SearchQuery { get; init; }
    public bool CanManage { get; init; }
    public IReadOnlyList<SportSubtypeListItemViewModel> Items { get; init; } = Array.Empty<SportSubtypeListItemViewModel>();
}

public sealed class SportSubtypeListItemViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int SportTypeId { get; init; }
    public string SportTypeTitle { get; init; } = string.Empty;
    public int SportId { get; init; }
    public string SportTitle { get; init; } = string.Empty;
    public int CompetitionCount { get; init; }
    public int SchoolCount { get; init; }
}

public sealed class SportSubtypeDetailsViewModel
{
    public int Id { get; init; }
    public string Title { get; init; } = string.Empty;
    public int SportTypeId { get; init; }
    public string SportTypeTitle { get; init; } = string.Empty;
    public int SportId { get; init; }
    public string SportTitle { get; init; } = string.Empty;
    public int CompetitionCount { get; init; }
    public int SchoolCount { get; init; }
    public bool CanManage { get; init; }
    public bool CanDelete { get; init; }
    public string? DeleteBlockedReason { get; init; }
    public IReadOnlyList<string> CompetitionTitles { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> SchoolTitles { get; init; } = Array.Empty<string>();
}

public sealed class SportSubtypeEditViewModel
{
    public int? Id { get; set; }

    [Display(Name = "Тип спорта")]
    [Required(ErrorMessage = "Выберите тип спорта.")]
    public int? SportTypeId { get; set; }

    [Display(Name = "Дисциплина")]
    [Required(ErrorMessage = "Введите название дисциплины.")]
    [StringLength(255)]
    public string Title { get; set; } = string.Empty;

    public IReadOnlyList<SelectListItem> SportTypeOptions { get; set; } = Array.Empty<SelectListItem>();
}
