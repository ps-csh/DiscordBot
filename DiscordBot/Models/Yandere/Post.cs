using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiscordBot.Models.Yandere
{
    [Table("YanderePosts")]
    public class Post : DbObject
    {
        public string PostId { get; set; }
        public int? PoolId { get; set; }

        public string FileUrl { get; set; }

        // Ratings are character codes (s - Safe, q - Questionable, e - Explicit?)
        public string Rating { get; set; }

        public virtual ICollection<Tag> Tags { get; set; }
        public virtual Pool Pool { get; set; }
    }
}
