using System;
using System.Collections.Generic;

namespace Grade_project.Database.Models;

public partial class User
{
    public int UserId { get; set; }

    public string? Login { get; set; }

    public string? Email { get; set; }

    public string? Password { get; set; }

    public DateOnly? DateCreated { get; set; }

    public DateOnly? DateUpdate { get; set; }

    public bool IsEmailConfirmed { get; set; }

    public virtual ICollection<Tokenss> Tokensses { get; set; } = new List<Tokenss>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
