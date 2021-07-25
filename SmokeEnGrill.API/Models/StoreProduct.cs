namespace SmokeEnGrill.API.Models
{
    public class StoreProduct
    {
        public int Id { get; set; }
        public int StoreId { get; set; }
        public int ProductId { get; set; }
        public int Qty { get; set; }
        public int PendingQty { get; set; }
        public Product Product { get; set; }
        public Store Store { get; set; }
    }
}