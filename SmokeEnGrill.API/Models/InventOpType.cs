using System;
using System.Collections.Generic;

namespace SmokeEnGrill.API.Models
{
    public partial class InventOpType : BaseEntity
    {

  public enum TypeEnum
        {
            StockEntry = 1,
            StockTransfer = 2,
            StockAllocation = 3,
            StockExchange = 4,
            Failure = 5,
            Export = 6,
            ToRepair = 7,
            FromRepair = 8,
            Maintenance = 9,
            TabletData = 10,
            InventOp = 11,
            EmpMvtToEC = 12,
            TabletCorrection = 13,
            EcMvt = 14
        }
        public string Name { get; set; }
    }
}
