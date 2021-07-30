using System;

namespace SmokeEnGrill.API.Models
{
    public class OrderLineHistory : BaseEntity
  {
        public OrderLineHistory()
        {
            Cashed = false;
            Rejected = false;
            InsertDate = DateTime.Now;
            InsertUserId = 1;
            UpdateDate = DateTime.Now;
            UpdateUserId = 1;
        }

        public int OrderLineId { get; set; }
        public OrderLine OrderLine { get; set; }
        public int? FinOpId { get; set; }
        public FinOp FinOp { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public DateTime OpDate { get; set; }
        public string Action { get; set; }
        public decimal OldAmount { get; set; }
        public decimal NewAmount { get; set; }
        public decimal Delta { get; set; }
        public Boolean Cashed { get; set; }
        public Boolean Rejected { get; set; }
    }
}