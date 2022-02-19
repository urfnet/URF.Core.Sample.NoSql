using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace URF.Core.Sample.NoSql.Models
{
    public class Book
    {
        public Guid Id { get; set; }

        [BsonElement("Name")]
        public string BookName { get; set; } = null!;

        public decimal Price { get; set; }

        public string Category { get; set; } = null!;

        public string Author { get; set; } = null!;

        public List<Reviewer> Reviewers { get; set; } = null!;
    }
}
