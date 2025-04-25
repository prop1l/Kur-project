using System;
using System.Collections.Generic;

namespace Grade_project.Database.Models;

public partial class Group
{
    public int GroupId { get; set; }

    public int SpecialityId { get; set; }

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual Speciality Speciality { get; set; } = null!;
}
