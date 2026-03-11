using System;
using System.Collections.Generic;

namespace SportEvents.Web.Models.Db;

public partial class vParticipantsFull
{
    public int ParticipantId { get; set; }

    public int SchoolId { get; set; }

    public string SchoolTitle { get; set; } = null!;

    public string? SchoolDescription { get; set; }

    public int ContactId { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public int Age { get; set; }

    public bool Sex { get; set; }

    public string Phone { get; set; } = null!;

    public string? Email { get; set; }
}
