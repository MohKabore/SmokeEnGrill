namespace SmokeEnGrill.API.Models
{
    public class Menu : BaseEntity
    {
        public string Name { get; set; }
        public int UserTypeId { get; set; }
        public UserType UserType { get; set; }
    }
}