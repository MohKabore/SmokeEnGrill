﻿using System;
using System.Collections.Generic;

namespace SmokeEnGrill.API.Models
{
    public partial class StockMvt
    {

        public int Id { get; set; }
        public int InventOpTypeId { get; set; }
        public DateTime MvtDate { get; set; }
        public string RefNum { get; set; }
        public string Note { get; set; }
        public int? FromStoreId { get; set; }
        public int? FromEmployeeId { get; set; }
        public int? ToStoreId { get; set; }
        public int? ToEmployeeId { get; set; }
        public byte Status { get; set; }

        public  User FromEmployee { get; set; }
        public  Store FromStore { get; set; }
        public  InventOpType InventOpType { get; set; }
        public  User ToEmployee { get; set; }
        public  Store ToStore { get; set; }
        public int InsertUserId { get; set; }
        public User InsertUser { get; set; }
        // public  ICollection<StockMvtInventOp> StockMvtInventOp { get; set; }
    }
}
