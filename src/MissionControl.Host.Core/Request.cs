namespace MissionControl.Host.Core
{
    public class Request
    {
        public Request()
        {
            Args = new string[0];    
        }
        
        public string ClientId { get; set; }

        public string Command { get; set; }

        public string[] Args { get; set; }
    }
}