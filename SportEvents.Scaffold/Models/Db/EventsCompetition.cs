using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class EventsCompetition
{
    public int id { get; set; }

    public int idEvent { get; set; }

    public int idCompetition { get; set; }

    public virtual Competition idCompetitionNavigation { get; set; } = null!;

    public virtual Event idEventNavigation { get; set; } = null!;
}
