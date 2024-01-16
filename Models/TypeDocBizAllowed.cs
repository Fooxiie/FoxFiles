using SQLite;

namespace FoxFiles.Models
{
    public class TypeDocBizAllowed
    {
        [AutoIncrement, PrimaryKey]
        public int Id { get; set; }
        
        [NotNull]
        public int TypeDocumentId { get; set; }
        
        [NotNull]
        public int BizAllowedId { get; set; }
    }
}