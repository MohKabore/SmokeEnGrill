using System;

namespace SmokeEnGrill.API.Models
{
    public class SmsTemplate : BaseEntity
    {
        public SmsTemplate()
        {
            Internal = true;
        }

        public string Name { get; set; }
        public string Content { get; set; }
        public int SmsCategoryId { get; set; }
        public SmsCategory SmsCategory { get; set; }
        public Boolean Internal { get; set; }
    }
}