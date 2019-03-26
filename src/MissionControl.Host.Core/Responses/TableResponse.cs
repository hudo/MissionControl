using System.Collections.Generic;
using System.Linq;

namespace MissionControl.Host.Core.Responses
{
    /// <summary>
    /// Represents rows of data, displayed as a table
    /// </summary>
    public class TableResponse<T> : CliResponse where T:class
    {
        public TableResponse()
        {
            Rows = new List<T>();
        }

        /// <summary>
        /// Table description
        /// </summary>
        public string Description { get; set; }

        public List<T> Rows { get; set; }

        public override string Type => "table";
    }
}
