namespace URF.Core.Sample.NoSql.Api.Settings
{
    public interface IBookstoreDatabaseSettings
    {
        string AuthorsCollectionName { get; set; }
        string BooksCollectionName { get; set; }
        string ConnectionString { get; set; }
        string DatabaseName { get; set; }
    }
}
