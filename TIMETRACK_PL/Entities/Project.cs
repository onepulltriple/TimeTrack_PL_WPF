using System;
using System.Collections.Generic;

namespace TIMETRACK_PL.Entities;

public partial class Project
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public string Number { get; set; } = null!;

    public string? Description { get; set; }

    public bool IsArchived { get; set; }

    public virtual ICollection<Task> Tasks { get; set; } = new List<Task>();
}
