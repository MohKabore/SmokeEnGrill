using System;
using System.Collections.Generic;

namespace SmokeEnGrill.API.Models
{
    public partial class Store : BaseEntity
    {
        public string Name { get; set; }
        public int StoreTypeId { get; set; }
        public StoreType StoreType { get; set; }
        public int? StoreIdP { get; set; }
        public Store StoreP { get; set; }
        public int? DistrictId { get; set; }
        public District District { get; set; }
        public int UserId { get; set; }
        public User User { get; set; }
        public byte Status { get; set; }
    }
}
