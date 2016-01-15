using System.Collections.Generic;

namespace ClientDirectory.Models
{
    public class DataTablesResponse
    {
        public IEnumerable<Client> Data { get; set; }
        public int Draw { get; set; }
        public string Error { get; set; }
        public int RecordsFiltered { get; set; }
        public int RecordsTotal { get; set; }
    }
}