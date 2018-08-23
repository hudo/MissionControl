using System;

namespace MissionControl.Host.Core
{
    public abstract class CliCommand
    {
        protected CliCommand()
        {
            Id = Guid.NewGuid(); 
        }
        
        public Guid Id { get; }
    }

    [CommandText("hello-world")]
    public class HelloWorldCommand : CliCommand
    {
        public string Name { get; set; }
    }
    
    public class CommandTextAttribute : Attribute
    {
        public CommandTextAttribute(string commandText)
        {
            CommandText = commandText;
        }
        
        public string CommandText { get; }
    }

   
}
