using System;
using System.Collections.Generic;

namespace SportEvents.Web.Models.Db;

public partial class vEventsCompetitionsFull
{
    public int EventId { get; set; }

    public string EventTitle { get; set; } = null!;

    public string? EventPhotoUrl { get; set; }

    public DateTime? EventDateStart { get; set; }

    public DateTime? EventDateEnd { get; set; }

    public int CompetitionId { get; set; }

    public string CompetitionTitle { get; set; } = null!;

    public string? CompetitionPhotoUrl { get; set; }

    public DateTime? CompetitionDateStart { get; set; }

    public DateTime? CompetitionDateEnd { get; set; }

    public int SportId { get; set; }

    public string SportTitle { get; set; } = null!;

    public int SportTypeId { get; set; }

    public string SportTypeTitle { get; set; } = null!;

    public int SportSubTypeId { get; set; }

    public string SportSubTypeTitle { get; set; } = null!;
}
