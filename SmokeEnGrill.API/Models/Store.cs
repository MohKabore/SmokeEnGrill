using System;
using System.Collections.Generic;

namespace SmokeEnGrill.API.Models
{
    public partial class Store
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public int StoreTypeId { get; set; }
        public int?  StorePId { get; set; }
        public byte Status { get; set; }
        public DateTime InsertDate { get; set; }
        public StoreType StoreType { get; set; }
        public Store    StoreP { get; set; }
    }
}
