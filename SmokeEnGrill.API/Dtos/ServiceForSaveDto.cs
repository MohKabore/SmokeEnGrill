using System;
using System.Collections.Generic;
using EducNotes.API.Models;

namespace EducNotes.API.Dtos
{
  public class ServiceForSaveDto
  {
    public int Id { get; set; }
    public string Name { get; set; }
    public string Comment { get; set; }
    public int ProductTypeId { get; set; }
    public decimal? Price { get; set; }
    public bool IsPaidCash { get; set; }
    public bool IsByLevel { get; set; }
    public bool IsByZone { get; set; }
    public bool IsPeriodic { get; set; }
    public int? PeriodicityId { get; set; }
    public int? PayableAtId { get; set; }
    public DateTime ServiceStartDate { get; set; }
    public string strStartDate { get; set; }
    public bool IsRequired { get; set; }
    public bool Active { get; set; }
    public byte DsplSeq { get; set; }
  }
}