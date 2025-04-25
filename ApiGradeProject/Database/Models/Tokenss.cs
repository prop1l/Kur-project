using System;
using System.Collections.Generic;

namespace ApiGradeProject.Database.Models;

public partial class Tokenss
{
    public int TokenId { get; set; }

    public string TokenValue { get; set; } = null!;

    public int UsersId { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime ExpiresAt { get; set; }

    public virtual User Users { get; set; } = null!;
}
