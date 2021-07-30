namespace SmokeEnGrill.API.Models
{
    public class BankAccount : BaseEntity
    {
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public byte Active { get; set; }
    }
}