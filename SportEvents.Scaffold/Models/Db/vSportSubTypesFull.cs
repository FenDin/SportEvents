using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class vSportSubTypesFull
{
    public int SportId { get; set; }

    public string SportTitle { get; set; } = null!;

    public int SportTypeId { get; set; }

    public string SportTypeTitle { get; set; } = null!;

    public int SportSubTypeId { get; set; }

    public string SportSubTypeTitle { get; set; } = null!;
}
