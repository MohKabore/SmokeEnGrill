using System;

namespace SmokeEnGrill.API.Models
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public int ProductTypeId { get; set; }
        public ProductType ProductType { get; set; }
        public decimal Price { get; set; }
        public int Qty { get; set; } = 0;
        public int PendingOrders { get; set; } = 0;
        public int CriticalQty { get; set; } = 0;
        public byte Status { get; set; }
    }
}