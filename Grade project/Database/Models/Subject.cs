using System;
using System.Collections.Generic;

namespace Grade_project.Database.Models;

public partial class Subject
{
    public int SubjectId { get; set; }

    public string SubjectName { get; set; } = null!;

    public int TeacherId { get; set; }

    public virtual Teacher Teacher { get; set; } = null!;
}
