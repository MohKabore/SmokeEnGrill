using System.Collections.Generic;

namespace SmokeEnGrill.API.Dtos
{
    public class NewMenuDto
    {
        public string Name { get; set; }
        public byte Status { get; set; }
        public List<ProductWithQtyDto> Products { get; set; }
    }
}