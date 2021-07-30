using System.Collections.Generic;

namespace SmokeEnGrill.API.Dtos
{
    public class FoodMenuDto
    {
        public string Name { get; set; }
        public byte Status { get; set; }
        public List<FoodMenuProductDto> Products { get; set; }
    }
}