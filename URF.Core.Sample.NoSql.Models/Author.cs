using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace URF.Core.Sample.NoSql.Models
{
    public class Author
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = null!;

        public string Country { get; set; } = null!;
    }
}
