using System;

namespace SmokeEnGrill.API.Models
{
    public class FinOpOrderline : BaseEntity
    {
        public FinOpOrderline()
        {
            InsertDate = DateTime.Now;
            InsertUserId = 1;
            UpdateDate = DateTime.Now;
            UpdateUserId = 1;
        }

        public int? InvoiceId { get; set; }
        public Invoice Invoice { get; set; }
        public int FinOpId { get; set; }
        public FinOp FinOp { get; set; }
        public int OrderLineId { get; set; }
        public OrderLine OrderLine { get; set; }
        public decimal Amount { get; set; }
    }
}