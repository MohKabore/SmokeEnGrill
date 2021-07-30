namespace SmokeEnGrill.API.Models
{
    public class District : BaseEntity
    {
        public string Name { get; set; }
        public int CityId { get; set; }
        public City City { get; set; }
    }
}