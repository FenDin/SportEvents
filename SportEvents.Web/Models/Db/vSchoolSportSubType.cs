using System;
using System.Collections.Generic;

namespace SportEvents.Web.Models.Db;

public partial class vSchoolSportSubType
{
    public int SchoolId { get; set; }

    public string SchoolTitle { get; set; } = null!;

    public string? SchoolDescription { get; set; }

    public int SportId { get; set; }

    public string SportTitle { get; set; } = null!;

    public int SportTypeId { get; set; }

    public string SportTypeTitle { get; set; } = null!;

    public int SportSubTypeId { get; set; }

    public string SportSubTypeTitle { get; set; } = null!;
}
