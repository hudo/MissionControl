using System;

namespace MissionControl.Host.Core.Contracts
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CliCommandAttribute : Attribute
    {
        public CliCommandAttribute(string commandText, string description = "")
        {
            CommandText = commandText;
            Description = description;
        }
        
        public string CommandText { get; }

        public string Description { get; }
    }
}