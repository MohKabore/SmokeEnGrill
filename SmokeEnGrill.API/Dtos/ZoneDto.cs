using System;
using System.Collections.Generic;

namespace EducNotes.API.Dtos
{
  public class ZoneDto
  {
    public ZoneDto()
    {
      Locations = new List<LocationZoneDto>();
      Used = false;
      Deleted = false;
    }
    
    public int Id { get; set; }
    public string Name { get; set; }
    public string NameWithLocations { get; set; }
    public Boolean Used { get; set; }
    public Boolean Deleted { get; set; }
    public List<LocationZoneDto> Locations { get; set; }
  }
}