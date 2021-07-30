using System;

namespace SmokeEnGrill.API.Models
{
    public class EmailTemplate : BaseEntity
    {
    public EmailTemplate()
    {
      Internal = true;
    }

    public string Name { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public int EmailCategoryId { get; set; }
    public EmailCategory EmailCategory { get; set; }
    public Boolean Internal { get; set; }
    }
}