using URF.Core.Abstractions;
using URF.Core.Sample.NoSql.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Mongo
{
    public class BookstoreUnitOfWork : IBookstoreUnitOfWork
    {
        public BookstoreUnitOfWork(IDocumentRepository<Author> authorsRepository,
            IDocumentRepository<Book>booksRepository)
        {
            AuthorsRepository = authorsRepository;
            BooksRepository = booksRepository;
        }

        public IDocumentRepository<Author> AuthorsRepository { get; }

        public IDocumentRepository<Book> BooksRepository { get; }
    }
}
