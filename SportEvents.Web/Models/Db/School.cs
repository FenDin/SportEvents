using System;
using System.Collections.Generic;

namespace SportEvents.Web.Models.Db;

public partial class School
{
    public int id { get; set; }

    public string title { get; set; } = null!;

    public string? description { get; set; }

    public virtual ICollection<Participant> Participants { get; set; } = new List<Participant>();

    public virtual ICollection<SchoolsSportsSubtype> SchoolsSportsSubtypes { get; set; } = new List<SchoolsSportsSubtype>();
}
