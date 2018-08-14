namespace MissionControl.Host.AspnetCore
{
    public class McOptions
    {
        public McOptions()
        {
            Url = "mc";
        }
        
        /// <summary>
        /// Url that opens Mission Control.
        /// Default is 'mc/'
        /// </summary>
        public string Url { get; set; }
    }
}