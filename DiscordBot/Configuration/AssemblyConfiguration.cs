using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Configuration
{
    public class AssemblyConfiguration
    {
        /// <summary>
        /// A list of assemblies to load
        /// </summary>
        public List<Assembly> Assemblies { get; set; }

        public class Assembly
        {
            public string Name { get; set; }
            public string Type { get; set; }
        }
    }
}
