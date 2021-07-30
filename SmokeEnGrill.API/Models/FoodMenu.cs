using System.Collections.Generic;
using SmokeEnGrill.API.Dtos;

namespace SmokeEnGrill.API.Models
{
    public class FoodMenu : BaseEntity
    {
        public string Name { get; set; }
        public byte Status { get; set; }
        public List<FoodMenuProductDto> Products { get; set; }
    }
}