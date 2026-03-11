using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class ImagesUser
{
    public int id { get; set; }

    public int idImage { get; set; }

    public int idContact { get; set; }

    public virtual Contact idContactNavigation { get; set; } = null!;

    public virtual Image idImageNavigation { get; set; } = null!;
}
