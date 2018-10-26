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
        /// <param name="visible">Visible in list of registered commands. Default is true</param>
        public CliCommandAttribute(string commandText, string help = "", bool visible = true)
        {
            CommandText = commandText;
            Help = help;
            Visible = visible;
        }
        
        public string CommandText { get; }

        public string Help { get; }
        
        public bool Visible { get; }
    }
}