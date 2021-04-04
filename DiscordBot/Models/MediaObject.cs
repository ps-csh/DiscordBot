using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Models
{
    public class MediaObject : PostableObject
    {
        public string Url { get; set; }
        public string PhysicalUrl { get; set; }

        public MediaType Type { get; set; }

        public enum MediaType
        {
            Image = 0,
            Video,
            Audio,
            Website,
        }
    }
}
