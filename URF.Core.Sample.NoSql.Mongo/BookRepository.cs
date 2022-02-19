using MongoDB.Driver;
using URF.Core.Mongo;
using URF.Core.Sample.NoSql.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Mongo
{
    public class BookRepository : DocumentRepository<Book>, IBookRepository
    {
        public BookRepository(IMongoCollection<Book> collection) : base(collection) { }

        public async Task<Book> AddReviewer(string id, Reviewer reviewer, 
            CancellationToken cancellationToken = default)
        {
            var update = Builders<Book>.Update.Push(e => e.Reviewers, reviewer);
            return await Collection.FindOneAndUpdateAsync(e => e.Id == id, update, null, cancellationToken);
        }

        public async Task<Book> UpdateReviewer(string id, Reviewer reviewer,
            CancellationToken cancellationToken = default)
        {
            var filter = Builders<Book>.Filter;
            var bookReviewerFilter = filter.And(
              filter.Eq(x => x.Id, id),
              filter.ElemMatch(x => x.Reviewers, c => c.Name == reviewer.Name));
            var update = Builders<Book>.Update;
            var reviewerSetter = update.Set("Reviewers.$.Institute", reviewer.Institute);
            return await Collection.FindOneAndUpdateAsync<Book>(bookReviewerFilter, reviewerSetter, null, cancellationToken);
        }

        public async Task<Book> DeleteReviewer(string id, string name, CancellationToken cancellationToken = default)
        {
            var update = Builders<Book>.Update.PullFilter(p => p.Reviewers, f => f.Name == name);
            return await Collection.FindOneAndUpdateAsync(e => e.Id == id, update, null, cancellationToken);
        }
    }
}
