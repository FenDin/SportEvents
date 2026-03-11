using System;
using System.Collections.Generic;

namespace SportEvents.Web.Models.Db;

public partial class Sport
{
    public int id { get; set; }

    public string title { get; set; } = null!;

    public virtual ICollection<SportType> SportTypes { get; set; } = new List<SportType>();
}
