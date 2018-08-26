﻿namespace MissionControl.Host.Core
{
    public abstract class Request
    {
        public string ClientId { get; set; }

        public string Command { get; set; }

        public string[] Args { get; set; }
    }
}