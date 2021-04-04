using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    public class PostableObject : DbObject
    {
        public string? CreatedBy { get; set; }
        public DateTime? CreatedAt { get; set; }
        public int TimesPosted { get; set; }

        public virtual ICollection<Tag> Tags { get; set; } 
    }
}
