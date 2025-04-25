using System;
using System.Collections.Generic;

namespace ApiGradeProject.Database.Models;

public partial class Token
{
    public int? IdUser { get; set; }

    public string Token1 { get; set; } = null!;

    public virtual User? IdUserNavigation { get; set; }
}
