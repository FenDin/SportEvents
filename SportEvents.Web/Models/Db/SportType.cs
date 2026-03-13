using System;
using System.Collections.Generic;

namespace SportEvents.Web.Models.Db;

public partial class SportType
{
    public int id { get; set; }

    public int idSport { get; set; }

    public string title { get; set; } = null!;

    public virtual ICollection<SportSubtype> SportSubtypes { get; set; } = new List<SportSubtype>();

    public virtual Sport idSportNavigation { get; set; } = null!;
}
