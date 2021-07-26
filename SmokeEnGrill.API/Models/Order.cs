using System;
using System.Collections.Generic;

namespace SmokeEnGrill.API.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int OrderTypeId { get; set; }
        public int? ClientId { get; set; }
        public User Client { get; set; }
        public string OrderNum { get; set; }
        public string OrderLabel { get; set; }
        public DateTime OrderDate { get; set; }
        public int DeliveryMethodId { get; set; }
        public int PaymentTypeId { get; set; }
        public decimal SubTotal { get; set; }
        public PaymentType PaymentType { get; set; }
        public DeliveryMethod DeliveryMethod { get; set; }
        public OrderType OrderType { get; set; }

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
    public Boolean isReg { get; set; }
    public Boolean isNextReg { get; set; }
    public DateTime InsertDate { get; set; } = DateTime.Now;
    public int InsertUserId { get; set; }
    public User InsertUser { get; set; }
    public DateTime UpdateDate { get; set; } = DateTime.Now;
    public int UpdateUserId { get; set; }
    public User UpdateUser { get; set; }
    public List<OrderLine> Lines { get; set; }
    public byte Status { get; set; }

    }
}