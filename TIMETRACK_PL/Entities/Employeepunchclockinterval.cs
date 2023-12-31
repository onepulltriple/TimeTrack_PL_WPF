using System;
using System.Collections.Generic;

namespace TIMETRACK_PL.Entities;

public partial class Employeepunchclockinterval
{
    public int Id { get; set; }

    public DateTime StartTimeActual { get; set; }

    public DateTime? EndTimeActual { get; set; }

    public DateTime? StartTimePunchClockSoftware { get; set; }

    public DateTime? EndTimePunchClockSoftware { get; set; }
}
