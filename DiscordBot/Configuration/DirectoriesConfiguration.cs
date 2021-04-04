using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Configuration
{
    public class DirectoriesConfiguration
    {
        /// <summary>
        /// The folder that is first opened by the ImageSorterWindow
        /// </summary>
        public string DefaultImagesFolder { get; set; }

        /// <summary>
        /// The base folder that images are saved to when adding them to the database
        /// </summary>
        public string SavedImagesFolder { get; set; }
    }
}
