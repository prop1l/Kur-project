using System;
using System.Collections.Generic;

namespace Grade_project.Database.Models;

public partial class Token
{
    public int? IdUser { get; set; }

    public string Token1 { get; set; } = null!;

    public virtual User? IdUserNavigation { get; set; }
}
