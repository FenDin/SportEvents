using System;
using System.Collections.Generic;

namespace SportEvents.Scaffold.Models.Db;

public partial class vParticipantsFull
{
    public int ParticipantId { get; set; }

    public int SchoolId { get; set; }

    public string SchoolTitle { get; set; } = null!;

    public string? SchoolDescription { get; set; }

    public int ContactId { get; set; }

    public string Email { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string? Phone { get; set; }

    public string? PasswordHash { get; set; }
}
