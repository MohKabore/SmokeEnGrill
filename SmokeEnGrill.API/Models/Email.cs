using System;

namespace SmokeEnGrill.API.Models
{
    public class Email : BaseEntity
    {
        public Email()
        {
            TimeToSend = DateTime.Now;
            InsertDate =DateTime.Now;
        }

        public int EmailTypeId { get; set; }
        public EmailType EmailType { get; set; }
        public int? StudentId { get; set; }
        public User Student { get; set; }
        public int ToUserId { get; set; }
        public User ToUser { get; set; }
        public string ToAddress { get; set; }
        public string CCAddress { get; set; }
        public string BCCAddress { get; set; }
        public string FromAddress { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public DateTime TimeToSend { get; set; }
        public byte StatusFlag { get; set; }
        public int? OrderId { get; set; }
        public Order Order { get; set; }
    }
}