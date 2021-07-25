using System;

namespace SmokeEnGrill.API.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public byte Status { get; set; }
        public DateTime InsertDate { get; set; }
    }
}