using FoxORM;
using SQLite;

namespace FoxFiles.Models
{
    public class Document
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        
        public int IdTypeDocument { get; set; }
        
        [Ignore]
        public TypeDocument TypeDocument { get; set; }

        public async void LoadTypeDocument(FoxOrm orm)
        {
            TypeDocument = await orm.Query<TypeDocument>(IdTypeDocument);
        }
    }
}