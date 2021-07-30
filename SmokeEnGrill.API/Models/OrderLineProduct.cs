namespace SmokeEnGrill.API.Models
{
    public class OrderLineProduct : BaseEntity
    {
        public int MenuId { get; set; }
        public Menu Menu { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Qty { get; set; }
    }
}