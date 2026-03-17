using System;
using System.Collections.Generic;

namespace SportEvents.Web.Models.Db;

public partial class vContact
{
    public int ContactId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public DateTime BirthDate { get; set; }

    public bool Sex { get; set; }

    public string? Phone { get; set; }

    public string Email { get; set; } = null!;

    public string? PasswordHash { get; set; }
}
