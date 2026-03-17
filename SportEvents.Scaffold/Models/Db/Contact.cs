using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class Contact
{
    public int id { get; set; }

    public string firstname { get; set; } = null!;

    public string lastname { get; set; } = null!;

    public string? middlename { get; set; }

    public DateTime birthDate { get; set; }

    public bool sex { get; set; }

    public string? phone { get; set; }

    public string email { get; set; } = null!;

    public string? passwordHash { get; set; }

    public virtual ICollection<ImagesUser> ImagesUsers { get; set; } = new List<ImagesUser>();

    public virtual Participant? Participant { get; set; }

    public virtual User? User { get; set; }
}
