using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class ParticipantsCompetition
{
    public int id { get; set; }

    public int idCompetition { get; set; }

    public int idParticipant { get; set; }

    public virtual Competition idCompetitionNavigation { get; set; } = null!;

    public virtual Participant idParticipantNavigation { get; set; } = null!;
}
