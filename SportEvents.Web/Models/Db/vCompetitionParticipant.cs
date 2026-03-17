using System;
using System.Collections.Generic;

namespace SportEvents.Web.Models.Db;

public partial class vCompetitionParticipant
{
    public int CompetitionId { get; set; }

    public string CompetitionTitle { get; set; } = null!;

    public string? CompetitionDescription { get; set; }

    public DateTime? CompetitionDateStart { get; set; }

    public DateTime? CompetitionDateEnd { get; set; }

    public int SportId { get; set; }

    public string SportTitle { get; set; } = null!;

    public int SportTypeId { get; set; }

    public string SportTypeTitle { get; set; } = null!;

    public int SportSubTypeId { get; set; }

    public string SportSubTypeTitle { get; set; } = null!;

    public int ParticipantId { get; set; }

    public int SchoolId { get; set; }

    public string SchoolTitle { get; set; } = null!;

    public string? SchoolDescription { get; set; }

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
