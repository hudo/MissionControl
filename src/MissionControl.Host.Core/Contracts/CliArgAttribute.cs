using System;

namespace MissionControl.Host.Core.Contracts
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public class CliArgAttribute : Attribute
    {
        /// <summary>
        /// Property of a CLI command
        /// </summary>
        /// <param name="required">Is required</param>
        /// <param name="help">Text displayed when command is invoked with -help argument</param>
        /// <param name="skip">This property will not be used as command argument</param>
        public CliArgAttribute(bool required = false, string help = null, bool skip = false)
        {
            Required = required;
            Help = help;
            Skip = skip;
        }
        
        public bool Required { get; }
        public string Help { get; }
        public bool Skip { get; set; }

    }
}