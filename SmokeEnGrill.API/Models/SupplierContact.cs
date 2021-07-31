namespace SmokeEnGrill.API.Models
{
    public class SupplierContact : BaseEntity
    {
        public int SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public string LastName { get; set; }
        public string FirstName { get; set; }
        public string Mobile { get; set; }
        public string PhoneNumber { get; set; }
    }
}