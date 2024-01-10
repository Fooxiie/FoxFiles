using SQLite;

namespace FoxFiles.Models
{
    public class TypeDocument
    {
        [AutoIncrement]
        [PrimaryKey]
        public int Id { get; set; }
        
        [NotNull]
        public string TypeName { get; set; }

        /// <summary>
        /// Character Id of the author
        /// </summary>
        [NotNull]
        public int Author { get; set; }
    }
}