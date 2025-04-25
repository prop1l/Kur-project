using System;
using System.Collections.Generic;

namespace ApiGradeProject.Database.Models;

public partial class UserInfo
{
    public int? UserId { get; set; }

    public string? FirstName { get; set; }

    public string? SecondName { get; set; }

    public string? LastName { get; set; }

    public virtual User? User { get; set; }
}
