using System;
using System.Collections.Generic;

namespace SmokeEnGrill.API.Models
{
    public partial class InventOp : BaseEntity
    {
        public InventOp()
        {
            InsertDate = DateTime.Now;
        }

        public int? InventOpTypeId { get; set; }
        public DateTime OpDate { get; set; }
        public int? OrderLineId { get; set; }
        public OrderLine OrderLine { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }
        public decimal Qty { get; set; }
        public decimal OldQty { get; set; }
        public decimal Delta { get; set; }
        public int? FromStoreId { get; set; }
        public Store FromStore { get; set; }
        public int? ToStoreId { get; set; }
        public Store ToStore { get; set; }
        public int? FromEmployeeId { get; set; }
        public User FromEmployee { get; set; }
        public int? ToEmployeeId { get; set; }
        public User ToEmployee { get; set; }
        public string FormNum { get; set; }
        public byte Status { get; set; }
    }
}
