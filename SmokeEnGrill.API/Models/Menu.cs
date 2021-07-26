namespace SmokeEnGrill.API.Models
{
    public class Menu
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }
        public int? PhotoId { get; set; }
        public Photo Photo { get; set; }
    }
}