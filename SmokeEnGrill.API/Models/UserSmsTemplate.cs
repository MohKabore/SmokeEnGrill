namespace SmokeEnGrill.API.Models
{
    public class UserSmsTemplate : BaseEntity
    {
        public int ChildId { get; set; }
        public User Child { get; set; }
        public int ParentId { get; set; }
        public User Parent { get; set; }
        public int SmsTemplateId { get; set; }
        public SmsTemplate SmsTemplate { get; set; }
    }
}