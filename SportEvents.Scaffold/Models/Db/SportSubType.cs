using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class SportSubType
{
    public int id { get; set; }

    public int idSportType { get; set; }

    public string title { get; set; } = null!;

    public virtual ICollection<Competition> Competitions { get; set; } = new List<Competition>();

    public virtual ICollection<SchoolsSportsSubtype> SchoolsSportsSubtypes { get; set; } = new List<SchoolsSportsSubtype>();

    public virtual SportType idSportTypeNavigation { get; set; } = null!;
}
