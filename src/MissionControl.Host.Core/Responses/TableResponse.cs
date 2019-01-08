using System.Collections.Generic;
using System.Linq;

namespace MissionControl.Host.Core.Responses
{
    /// <summary>
    /// Represents rows of data, displayed as a table
    /// </summary>
    public class TableResponse : CliResponse
    {
        public TableResponse()
        {
            Rows = new List<string[]>();
        }

        /// <summary>
        /// Set header row data, flag first row as header
        /// </summary>
        /// <param name="header"></param>
        public TableResponse(params string[] header)
        {
            Rows = new List<string[]> {header};
            HasHeader = true;
        }

        /// <summary>
        /// First row is header
        /// </summary>
        public bool HasHeader { get; set; }

        /// <summary>
        /// Table description
        /// </summary>
        public string Description { get; set; }

        public List<string[]> Rows { get; set; }

        /// <summary>
        /// Columns indexes which will be formatted as numbers (aligned to right).
        /// Other columns are treated as strings by default. 
        /// </summary>
        public int[] NumberColumns { get; set; }

        public TableResponse AddRow(params string[] row)
        {
            Rows.Add(row);
            return this;
        }

        /// <summary>
        /// Max number of cells
        /// </summary>
        public int MaxNumberOfColumns => Rows.Max(row => row.Length);

        public override string Type => "table";
    }
}
