using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiscordBot.Models.Yandere
{
    [Table("YanderePools")]
    public class Pool : DbObject
    {
        public string PoolId { get; set; }
        public string Name { get; set; }

        public int PostCount { get; set; }

        // When the Pool was changed in Yande.re, not in this database
        public string CreatedAt { get; set; }
        public string UpdatedAt { get; set; }

        public virtual ICollection<Post> Posts { get; set; }
    }
}
