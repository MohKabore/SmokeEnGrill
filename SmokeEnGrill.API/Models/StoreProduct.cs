namespace SmokeEnGrill.API.Models
{
    public class StoreProduct : BaseEntity
    {
        public int StoreId { get; set; }
        public Store Store { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Qty { get; set; }
        public decimal PendingQty { get; set; }
    }
}