using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    public class Anime : DbObject
    {
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime AirDate { get; set; }
        public int Episodes { get; set; }
        public bool Watching { get; set; }
        public string Url { get; set; }

        //The Tag relevant tag for this anime, to make it easier to find related images
        public Tag Tag { get; set; }
    }
}
