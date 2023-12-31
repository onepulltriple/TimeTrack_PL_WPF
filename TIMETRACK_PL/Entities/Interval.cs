using System;
using System.Collections.Generic;

namespace TIMETRACK_PL.Entities;

public partial class Interval
{
    public int Id { get; set; }

    public DateTime StartTimeActual { get; set; }

    public DateTime? EndTimeActual { get; set; }

    public DateTime? StartTimeRounded { get; set; }

    public DateTime? EndTimeRounded { get; set; }

    public int TaskId { get; set; }

    public virtual Task Task { get; set; } = null!;
}
