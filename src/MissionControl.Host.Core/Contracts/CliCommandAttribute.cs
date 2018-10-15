using System;

namespace MissionControl.Host.Core.Contracts
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CliCommandAttribute : Attribute
    {
        public CliCommandAttribute(string commandText)
        {
            CommandText = commandText;
        }
        
        public string CommandText { get; }
    }
}