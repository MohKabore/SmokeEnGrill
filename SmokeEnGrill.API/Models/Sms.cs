using System;

namespace SmokeEnGrill.API.Models
{
    public class Sms : BaseEntity
    {
        public Sms()
        {
        StatusFlag = 0;
        NbTries = 0;
        validityPeriod = 1;
        }

        public int SmsTypeId { get; set; }
        public SmsType SmsType { get; set; }
        public string To { get; set; }
        public int ToUserId { get; set; }
        public User ToUser { get; set; }
        public string From { get; set; }
        public string Content { get; set; }
        public Boolean Binary { get; set; }
        public string ClientMsgId { get; set; }
        public string scheduledDeliveryTime { get; set; }
        public string UserDataHeader { get; set; }
        public int validityPeriod { get; set; }
        public string Charset { get; set; }
        public byte StatusFlag { get; set; }
        public byte NbTries { get; set; }

        public string res_ApiMsgId { get; set; }
        public Boolean res_Accepted { get; set; }
        public int res_ErrorCode { get; set; }
        public string res_Error { get; set; }
        public string res_ErrorDesc { get; set; }
        public string cb_IntegrationName { get; set; }
        public int cb_StatusCode { get; set; }
        public string cb_Status { get; set; }
        public string cb_StatusDesc { get; set; }
        public string cb_RequestId { get; set; }
        public string cb_TimeStamp { get; set; }
    }
}