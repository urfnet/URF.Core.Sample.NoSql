using URF.Core.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Abstractions
{
    public interface IBookstoreUnitOfWork
    {
        public IDocumentRepository<Author> AuthorsRepository { get; }

        public IBookRepository BooksRepository { get; }
    }
}
