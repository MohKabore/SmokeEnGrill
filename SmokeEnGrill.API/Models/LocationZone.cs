namespace SmokeEnGrill.API.Models
{
    public class LocationZone : BaseEntity
    {
        public int ZoneId { get; set; }
        public Zone Zone { get; set; }
        public int? DistrictId { get; set; }
        public District District { get; set; }
        public int? CityId { get; set; }
        public City City { get; set; }
        public int? CountryId { get; set; }
        public Country Country { get; set; }
    }
}