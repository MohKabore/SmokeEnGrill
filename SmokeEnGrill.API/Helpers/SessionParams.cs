using System;

namespace SmokeEnGrill.API.Helpers
{
    public class SessionParams
    {
        public int ScheduleId { get; set; }
        public DateTime SessionDate { get; set; }
        public string Comment { get; set; }
    }
}