using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ProductForConfigDto
  {
    public ProductForConfigDto()
    {
      ZonePrices = new List<ProductZoneDto>();
    }

    public int Id { get; set; }
    public string Name { get; set; }
    public string Comment { get; set; }
    public int ProductTypeId { get; set; }
    public string TypeName { get; set; }
    public decimal Price { get; set; }
    public Boolean IsPaidCash { get; set; }
    public Boolean IsByLevel { get; set; }
    public Boolean IsByZone { get; set; }
    public Boolean IsPeriodic { get; set; }
    public int PeriodicityId { get; set; }
    public string PeriodicityName { get; set; }
    public byte PeriodicityNbDays { get; set; }
    public DateTime ServiceStartDate { get; set; }
    public string strStartDate { get; set; }
    public int PayableAtId { get; set; }
    public string PayableAtName { get; set; }
    public int PayableAtDayCount { get; set; }
    public Boolean IsRequired { get; set; }
    public Boolean Active { get; set; }
    public byte DspSeq { get; set; }
    public decimal LevelPrice { get; set; } = 0;
    public decimal ZonePrice { get; set; } = 0;
    public List<ProductZoneDto> ZonePrices { get; set; }
  }
}