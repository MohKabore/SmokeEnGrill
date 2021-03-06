using System;

namespace SmokeEnGrill.API.Models
{
    public class OrderLine : BaseEntity
    {
        public int OrderId { get; set; }
        public Order Order { get; set; }
        public string OrderLineLabel { get; set; }
        public int? MenuId { get; set; }
        public Menu Menu { get; set; }
        public int? ProductId { get; set; }
        public Product Product { get; set; }
        public decimal ProductFee { get; set; }
        public decimal Qty { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalHT { get; set; }
        public decimal Discount { get; set; }
        public decimal AmountHT { get; set; }
        public decimal TVA { get; set; }
        public decimal TVAAmount { get; set; }
        public decimal AmountTTC { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime Validity { get; set; }
        public Boolean Validated { get; set; }
        public Boolean Paid { get; set; }
        public Boolean Expired { get; set; }
        public Boolean OverDue { get; set; }
        public Boolean Cancelled { get; set; }
        public Boolean Completed { get; set; }
        public Boolean Active { get; set; }
    }
}