using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class Competition
{
    public int id { get; set; }

    public int idSportSubType { get; set; }

    public string title { get; set; } = null!;

    public string? description { get; set; }

    public DateTime? dateStart { get; set; }

    public DateTime? dateEnd { get; set; }

    public virtual ICollection<EventsCompetition> EventsCompetitions { get; set; } = new List<EventsCompetition>();

    public virtual ICollection<ParticipantsCompetition> ParticipantsCompetitions { get; set; } = new List<ParticipantsCompetition>();

    public virtual SportSubType idSportSubTypeNavigation { get; set; } = null!;
}
