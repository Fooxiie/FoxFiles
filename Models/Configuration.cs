using SQLite;

namespace FoxFiles.Models
{
    public class Configuration
    {
        [PrimaryKey]
        public string configCode { get; set; }

        public string value { get; set; }
    }
}