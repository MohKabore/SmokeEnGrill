using System;

namespace SmokeEnGrill.API.Dtos
{
    public class NewStoreDto
    {
        public NewStoreDto()
        {
            Status = 1;
            InsertDate = DateTime.Now;

        }

        public string Name { get; set; }
        public int StoreTypeId { get; set; }
        public int? StorePId { get; set; }
        public byte Status { get; set; }
        public DateTime InsertDate { get; set; }
    }

}