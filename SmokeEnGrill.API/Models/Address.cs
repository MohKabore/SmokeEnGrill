namespace SmokeEnGrill.API.Models
{
    public class Address : BaseEntity
    {
        public string Name { get; set; }
        public int? CountryId { get; set; }
        public Country Country { get; set; }
        public int? CityId { get; set; }
        public City City { get; set; }
        public int DistrictId { get; set; }
        public District District { get; set; }
    }
}