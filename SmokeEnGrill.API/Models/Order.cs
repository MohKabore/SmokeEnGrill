using System;
using System.Collections.Generic;

namespace SmokeEnGrill.API.Models
{
    public class Order : BaseEntity
    {
        public int OrderTypeId { get; set; }
        public OrderType OrderType { get; set; }
        public int? ClientId { get; set; }
        public User Client { get; set; }
        public int? SupplierId { get; set; }
        public Supplier Supplier { get; set; }
        public string OrderNum { get; set; }
        public string OrderLabel { get; set; }
        public DateTime OrderDate { get; set; }
        public int DeliveryMethodId { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public int PaymentTypeId { get; set; }
        public PaymentType PaymentType { get; set; }
        public int FromStoreId { get; set; }
        public Store Store { get; set; }
        public decimal SubTotal { get; set; }
        public DateTime Deadline { get; set; }
        public DateTime Validity { get; set; }
        public int? ShippingAddressId { get; set; }
        public Address ShippingAddress { get; set; }
        public int? BillingAddressId { get; set; }
        public Address BillingAddress { get; set; }
        public decimal TotalHT { get; set; }
        public decimal Discount { get; set; }
        public decimal AmountHT { get; set; }
        public decimal TVA { get; set; }
        public decimal TVAAmount { get; set; }
        public decimal AmountTTC { get; set; }
        public Boolean Validated { get; set; }
        public Boolean Expired { get; set; }
        public Boolean Cancelled { get; set; }
        public Boolean Overdue { get; set; }
        public Boolean Paid { get; set; }
        public Boolean Completed { get; set; }
        public byte Status { get; set; }
        public List<OrderLine> Lines { get; set; }
    }
}