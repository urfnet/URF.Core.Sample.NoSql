namespace URF.Core.Sample.NoSql.Api.Configuration
{
    public class BookstoreDatabaseSettings
    {
        public string AuthorsCollectionName { get; set; } = null!;
        public string BooksCollectionName { get; set; } = null!;
        public string ConnectionString { get; set; } = null!;
        public string DatabaseName { get; set; } = null!;
    }
}
