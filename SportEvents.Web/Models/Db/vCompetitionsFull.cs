using System;
using System.Collections.Generic;

namespace SportEvents.Web.Models.Db;

public partial class vCompetitionsFull
{
    public int CompetitionId { get; set; }

    public string CompetitionTitle { get; set; } = null!;

    public string? CompetitionDescription { get; set; }

    public DateTime? CompetitionDateStart { get; set; }

    public DateTime? CompetitionDateEnd { get; set; }

    public int SportId { get; set; }

    public string SportTitle { get; set; } = null!;

    public int SportTypeId { get; set; }

    public string SportTypeTitle { get; set; } = null!;

    public int SportSubTypeId { get; set; }

    public string SportSubTypeTitle { get; set; } = null!;
}
