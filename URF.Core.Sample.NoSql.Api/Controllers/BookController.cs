using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using URF.Core.Mongo;
using URF.Core.Sample.NoSql.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        public IBookstoreUnitOfWork UnitOfWork { get; }

        public BookController(IBookstoreUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        // GET: api/Book
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Get()
        {
            var result = await UnitOfWork.BooksRepository
                .Queryable()
                .OrderBy(e => e.BookName)
                .ToListAsync();
            return Ok(result);
        }

        // GET: api/Book/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Book>> Get(string id)
        {
            var result = await UnitOfWork.BooksRepository.FindOneAsync(e => e.Id == id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/Book
        [HttpPost]
        public async Task<ActionResult<Book>> Post([FromBody] Book value)
        {
            var result = await UnitOfWork.BooksRepository.InsertOneAsync(value);
            return CreatedAtAction(nameof(Get), new { id = value.Id }, result);
        }

        // PUT: api/Book/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> Put(string id, [FromBody] Book value)
        {
            if (string.Compare(id, value.Id, true) != 0) return BadRequest();
            var result = await UnitOfWork.BooksRepository.FindOneAndReplaceAsync(e => e.Id == id, value);
            return Ok(result);
        }

        // DELETE: api/Book/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var count = await UnitOfWork.BooksRepository.DeleteOneAsync(e => e.Id == id);
            if (count == 0) return NotFound();
            return NoContent();
        }

        // PUT: api/Book/AddReviewer/5
        [HttpPut("AddReviewer/{id}")]
        public async Task<ActionResult<Book>> AddReviewer(string id, [FromBody] Reviewer reviewer)
        {
            var result = await UnitOfWork.BooksRepository.AddReviewer(id, reviewer);
            return Ok(result);
        }

        // PUT: api/Book/UpdateReviewer/5
        [HttpPut("UpdateReviewer/{id}")]
        public async Task<ActionResult<Book>> UpdateReviewer(string id, [FromBody] Reviewer reviewer)
        {
            var result = await UnitOfWork.BooksRepository.UpdateReviewer(id, reviewer);
            return Ok(result);
        }

        // DELETE: api/Book/5/Reviewer/James Wood
        [HttpDelete("{id}/Reviewer/{name}")]
        public async Task<IActionResult> DeleteReviewer(string id, string name)
        {
            var result = await UnitOfWork.BooksRepository.DeleteReviewer(id, name);
            return Ok(result);
        }
    }
}