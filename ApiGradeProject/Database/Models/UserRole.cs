using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace ApiGradeProject.Database.Models;

public partial class UserRole
{
    public int UserRoleId { get; set; }

    public int UserId { get; set; }

    public int RoleId { get; set; }
    [JsonPropertyName("role")]

    public virtual Role Role { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
