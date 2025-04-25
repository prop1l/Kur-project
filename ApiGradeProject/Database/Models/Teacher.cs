using System;
using System.Collections.Generic;

namespace ApiGradeProject.Database.Models;

public partial class Teacher
{
    public int TeacherId { get; set; }

    public string FirstName { get; set; } = null!;

    public string? MiddleName { get; set; }

    public string LastName { get; set; } = null!;

    public DateOnly DateBirth { get; set; }

    public virtual ICollection<Rating> Ratings { get; set; } = new List<Rating>();

    public virtual ICollection<Subject> Subjects { get; set; } = new List<Subject>();
}
