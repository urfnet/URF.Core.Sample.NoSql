using URF.Core.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Abstractions
{
    public interface IBookRepository : IDocumentRepository<Book>
    {
        Task<Book> AddReviewer(Guid id, Reviewer reviewer,
            CancellationToken cancellationToken = default);

        Task<Book> UpdateReviewer(Guid id, Reviewer reviewer,
            CancellationToken cancellationToken = default);

        Task<Book> DeleteReviewer(Guid id, string name,
            CancellationToken cancellationToken = default);
    }
}
