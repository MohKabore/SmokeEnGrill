using System.Collections.Generic;

namespace SmokeEnGrill.API.Models
{
    public class UserType : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<User> Users { get; set; }
    }
}