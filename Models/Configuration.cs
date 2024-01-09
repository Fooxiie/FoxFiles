using SQLite;

namespace FoxFiles.Models
{
    public class Configuration
    {
        [PrimaryKey]
        [AutoIncrement]
        public int Id { get; set; }
        
        public string configCode { get; set; }

        public string value { get; set; }
    }
}