using System;
using System.Collections.Generic;

namespace TIMETRACK_PL.Entities;

public partial class Task
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string Description { get; set; }

    public int ProjectId { get; set; }

    public virtual ICollection<Interval> Intervals { get; set; } = new List<Interval>();

    public virtual Project Project { get; set; }
}
