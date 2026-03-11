using System;
using System.Collections.Generic;

namespace SportEvents.Web.Models.Db;

public partial class Role
{
    public int id { get; set; }

    public string title { get; set; } = null!;

    public virtual ICollection<User> Users { get; set; } = new List<User>();
}
