using System;

namespace SmokeEnGrill.API.Models
{
    public class Token : BaseEntity
    {
        public Token()
        {
            Internal = true;
        }

        public string Name { get; set; }
        public string TokenString { get; set; }
        public Boolean Internal { get; set; }
        public int? TokenTypeId { get; set; }
        public TokenType TokenType { get; set; }
    }
}