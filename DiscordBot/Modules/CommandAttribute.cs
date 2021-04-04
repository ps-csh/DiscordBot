using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordBot.Modules
{
    //TODO: Can custom attributes be used to force a method signature?
    //Example: void Method(int x)   have the attribute only work on Action<int>
    [System.AttributeUsage(AttributeTargets.Method)]
    public class CommandAttribute : System.Attribute
    {
        public string Name { get; set; }

        public CommandAttribute(string name)
        {
            Name = name;
        }
    }
}
