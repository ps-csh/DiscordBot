using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    public class Video : PostableObject
    {
        public string Name { get; set; }

        public string Url { get; set; }
        public string PhysicalUrl { get; set; }

        public long Length { get; set; }
        public string Extension { get; set; }
        public long FileSize { get; set; }
    }
}
