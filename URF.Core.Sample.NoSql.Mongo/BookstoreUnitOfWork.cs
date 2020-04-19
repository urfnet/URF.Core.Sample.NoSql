using URF.Core.Abstractions;
using URF.Core.Sample.NoSql.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Mongo
{
    public class BookstoreUnitOfWork : IBookstoreUnitOfWork
    {
        public BookstoreUnitOfWork(IDocumentRepository<Author> authorsRepository,
            IBookRepository booksRepository)
        {
            AuthorsRepository = authorsRepository;
            BooksRepository = booksRepository;
        }

        public IDocumentRepository<Author> AuthorsRepository { get; }

        public IBookRepository BooksRepository { get; }
    }
}
