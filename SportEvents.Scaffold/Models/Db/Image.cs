using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class Image
{
    public int id { get; set; }

    public string? url { get; set; }

    public string? title { get; set; }

    public string? description { get; set; }

    public DateTime dateCreated { get; set; }

    public virtual ICollection<ImagesUser> ImagesUsers { get; set; } = new List<ImagesUser>();
}
