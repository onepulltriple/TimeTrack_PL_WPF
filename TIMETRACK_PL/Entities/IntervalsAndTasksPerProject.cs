using System;
using System.Collections.Generic;

namespace TIMETRACK_PL.Entities;

public partial class IntervalsAndTasksPerProject
{
    public int ProjectId { get; set; }

    public DateTime ActualStartTime { get; set; }

    public DateTime? ActualEndTime { get; set; }

    public string TaskName { get; set; }

    public string TaskDescription { get; set; }
}
