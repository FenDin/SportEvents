using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class vUsersFull
{
    public int UserId { get; set; }

    public int RoleId { get; set; }

    public string RoleTitle { get; set; } = null!;

    public int ContactId { get; set; }

    public string? Email { get; set; }

    public string Phone { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string? PasswordHash { get; set; }
}
