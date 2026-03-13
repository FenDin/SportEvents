using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class Participant
{
    public int id { get; set; }

    public int? idSchool { get; set; }

    public int idContact { get; set; }

    public virtual ICollection<ParticipantsCompetition> ParticipantsCompetitions { get; set; } = new List<ParticipantsCompetition>();

    public virtual Contact idContactNavigation { get; set; } = null!;

    public virtual School? idSchoolNavigation { get; set; }
}
