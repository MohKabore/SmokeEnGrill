using System;
using System.Collections.Generic;
using SmokeEnGrill.API.Models;

namespace SmokeEnGrill.API.Dtos
{
    public class StockAllocationDto
    {
        public int? FromStoreId { get; set; }
        public int? ToStoreId { get; set; }
        public int? FromEmployeeId { get; set; }
        public int? ToEmployeeId { get; set; }
        public int? RegionId { get; set; }
        public int? DepartmentId { get; set; }
        public DateTime  Mvtdate{ get; set; }
        public string RefNum { get; set; }
    }
}