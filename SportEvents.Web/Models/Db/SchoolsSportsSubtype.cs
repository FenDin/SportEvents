using System;
using System.Collections.Generic;

namespace SportEvents.Web.Models.Db;

public partial class SchoolsSportsSubType
{
    public int id { get; set; }

    public int idSportSubType { get; set; }

    public int idSchool { get; set; }

    public virtual School idSchoolNavigation { get; set; } = null!;

    public virtual SportSubtype idSportSubTypeNavigation { get; set; } = null!;
}
