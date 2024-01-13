using System;
using System.Collections.Generic;

namespace TIMETRACK_PL.Entities;

public partial class IntervalsAndTasksPerProject
{
    public int ProjectId { get; set; }

    public int IntervalId { get; set; }

    public DateTime ActualStartTime { get; set; }

    public DateTime? ActualEndTime { get; set; }

    public int TaskId { get; set; }

    public string TaskName { get; set; }

    public string TaskDescription { get; set; }
}
