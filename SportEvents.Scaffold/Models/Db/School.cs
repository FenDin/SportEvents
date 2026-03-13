using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class School
{
    public int id { get; set; }

    public string title { get; set; } = null!;

    public string? description { get; set; }

    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();

    public virtual ICollection<SchoolsSportsSubType> SchoolsSportsSubTypes { get; set; } = new List<SchoolsSportsSubType>();
}
