using System;

namespace SmokeEnGrill.API.Dtos
{
    public class NewProductDto
    {
        public NewProductDto()
        {
            InsertDate = DateTime.Now;
            Status =1;
        }

        public string Name { get; set; }
        public byte Status { get; set; }
        public DateTime InsertDate { get; set; }
    }
}