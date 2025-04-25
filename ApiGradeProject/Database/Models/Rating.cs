using System;
using System.Collections.Generic;

namespace ApiGradeProject.Database.Models;

public partial class Rating
{
    public int RatingId { get; set; }

    public int GroupId { get; set; }

    public int TeacherId { get; set; }

    public decimal? Assessment { get; set; }

    public virtual Group Group { get; set; } = null!;

    public virtual Teacher Teacher { get; set; } = null!;
}
