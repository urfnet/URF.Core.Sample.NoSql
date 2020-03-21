namespace URF.Core.Sample.NoSql.Api.Settings
{
    public class BookstoreDatabaseSettings : IBookstoreDatabaseSettings
    {
        public string AuthorsCollectionName { get; set; }
        public string BooksCollectionName { get; set; }
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
    }
}
