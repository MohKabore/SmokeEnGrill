using System;

namespace SmokeEnGrill.API.Models
{
    public class MenuItem : BaseEntity
    {
        public MenuItem()
        {
        IsAlwaysEnabled = false;
        }

        public int MenuId { get; set; }
        public Menu Menu { get; set; }
        public int? ParentMenuId { get; set; }
        public MenuItem ParentMenu { get; set; }
        public string Name { get; set; }
        public string DisplayName { get; set; }
        public string Url { get; set; }
        public Boolean IsAlwaysEnabled { get; set; }
        public byte DsplSeq { get; set; }
    }
}