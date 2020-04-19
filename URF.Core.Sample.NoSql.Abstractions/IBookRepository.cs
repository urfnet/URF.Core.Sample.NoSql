using System.Threading;
using System.Threading.Tasks;
using URF.Core.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Abstractions
{
    public interface IBookRepository : IDocumentRepository<Book>
    {
        Task<Book> AddReviewer(string id, Reviewer reviewer,
            CancellationToken cancellationToken = default);

        Task<Book> UpdateReviewer(string id, Reviewer reviewer,
            CancellationToken cancellationToken = default);

        Task<Book> DeleteReviewer(string id, string name,
            CancellationToken cancellationToken = default);
    }
}
