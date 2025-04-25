using System;
using System.Collections.Generic;

namespace ApiGradeProject.Database.Models;

public partial class Speciality
{
    public int SpecialityId { get; set; }

    public string SpecName { get; set; } = null!;

    public DateTime? DateFormation { get; set; }

    public DateTime? DateDisbandment { get; set; }

    public virtual ICollection<Group> Groups { get; set; } = new List<Group>();
}
