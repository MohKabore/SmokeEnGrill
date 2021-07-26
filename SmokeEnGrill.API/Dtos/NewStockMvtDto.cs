using System;
using System.Collections.Generic;

namespace SmokeEnGrill.API.Dtos
{
    public class NewStockMvtDto
    {
        public NewStockMvtDto()
        {
            Status =1;
        }

        public int InventOpTypeId { get; set; }
        public DateTime MvtDate { get; set; }
        public string RefNum { get; set; }
        public string Note { get; set; }
        public int? FromStoreId { get; set; }
        public int? FromEmployeeId { get; set; }
        public int? ToStoreId { get; set; }
        public int? ToEmployeeId { get; set; }
        public byte Status { get; set; }
        public int? InsertUserId { get; set; }
        public List<ProductWithQtyDto> Products { get; set; }
    }
}