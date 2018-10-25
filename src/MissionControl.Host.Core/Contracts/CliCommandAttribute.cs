using System;

namespace MissionControl.Host.Core.Contracts
{
    [AttributeUsage(AttributeTargets.Class)]
    public class CliCommandAttribute : Attribute
    {
        /// <summary>
        /// Represents CLI command
        /// </summary>
        /// <param name="commandText">Name used to invoke this command</param>
        /// <param name="help">Description of command</param>
        public CliCommandAttribute(string commandText, string help = "")
        {
            CommandText = commandText;
            Help = help;
        }
        
        public string CommandText { get; }

        public string Help { get; }
    }
}