using System;

namespace SmokeEnGrill.API.Models
{
    public class BaseEntity
    {
      public BaseEntity()
      {
        InsertUserId = 1;
        UpdateUserId = 1;
        InsertDate = DateTime.Now;
        UpdateDate = DateTime.Now;
        Version = Guid.NewGuid().ToString();
      }
      public int Id { get; set; }
      public DateTime InsertDate { get; set; }
      public int InsertUserId { get; set; }
      // public User InsertUser { get; set; }
      public DateTime UpdateDate { get; set; }
      public int UpdateUserId { get; set; }
      // public User UpdateUser { get; set; }
      public string Version { get; set; }
    }
}