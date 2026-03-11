using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class SportType
{
    public int id { get; set; }

    public int idSport { get; set; }

    public string title { get; set; } = null!;

    public virtual ICollection<SportSubType> SportSubTypes { get; set; } = new List<SportSubType>();

    public virtual Sport idSportNavigation { get; set; } = null!;
}
