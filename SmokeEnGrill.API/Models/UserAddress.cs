namespace SmokeEnGrill.API.Models
{
    public class UserAddress : BaseEntity
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int AddressId { get; set; }
        public Address Address { get; set; }
        public string Note { get; set; }
    }
}