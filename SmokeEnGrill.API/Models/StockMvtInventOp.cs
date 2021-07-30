namespace SmokeEnGrill.API.Models
{
    public class StockMvtInventOp : BaseEntity
    {
        public int InventOpId { get; set; }
        public InventOp InventOp { get; set; }
        public int StockMvtId { get; set; }
        public StockMvt StockMvt { get; set; }
    }
}