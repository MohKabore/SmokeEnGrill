namespace SmokeEnGrill.API.Models
{
    public class StoreProduct
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public decimal Qty { get; set; }
        public decimal PendingQty { get; set; }
        public Product Product { get; set; }
        public Store Store { get; set; }
    }
}