using DiscordBot.Models;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DiscordBot.Models
{
    public class Image : PostableObject
    {
        public string? Url { get; set; }
        public string? PhysicalUrl { get; set; }

        public int Width { get; set; }
        public int Height { get; set; }
        public string? Extension { get; set; }
        public long FileSize { get; set; }
        //[NotMapped]
    }
}
