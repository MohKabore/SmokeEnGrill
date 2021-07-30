namespace SmokeEnGrill.API.Models
{
    public class PayableAt : BaseEntity
    {
        public string Name { get; set; }
        public int DayCount { get; set; }
    }
}