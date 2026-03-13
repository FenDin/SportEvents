using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class SportSubtype
{
    public int id { get; set; }

    public int idSportType { get; set; }

    public string title { get; set; } = null!;

    public virtual ICollection<Competition> Competitions { get; set; } = new List<Competition>();

    public virtual ICollection<SchoolsSportsSubType> SchoolsSportsSubTypes { get; set; } = new List<SchoolsSportsSubType>();

    public virtual SportType idSportTypeNavigation { get; set; } = null!;
}
