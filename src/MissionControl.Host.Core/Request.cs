namespace MissionControl.Host.Core
{
    public class Request
    {
        public Request()
        {
            Args = new string[0];    
        }
        
        public string CorrelationId { get; set; }

        public string Command { get; set; }

        public string[] Args { get; set; }
    }
}