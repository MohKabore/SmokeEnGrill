namespace SmokeEnGrill.API.Dtos
{
    public class FoodMenuProductDto
    {
        public int ProductId { get; set; }
        public decimal Qty { get; set; }
        public decimal? PendingQty { get; set; }
    }
}