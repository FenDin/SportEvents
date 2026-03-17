using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class User
{
    public int id { get; set; }

    public int? idContact { get; set; }

    public int? idRole { get; set; }

    public DateTime dateCreated { get; set; }

    public virtual Contact? idContactNavigation { get; set; }

    public virtual Role? idRoleNavigation { get; set; }
}
