using System;

namespace MissionControl.Host.Core.Contracts
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CliArgAttribute : Attribute
    {
        public CliArgAttribute(bool required = false, string help = null)
        {
            Required = required;
            Help = help;
        }
        
        public bool Required { get; }
        public string Help { get; }
    }
}