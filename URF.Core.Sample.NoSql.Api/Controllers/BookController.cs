using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using URF.Core.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : ControllerBase
    {
        public IDocumentRepository<Book> Repository { get; }

        public BookController(IDocumentRepository<Book> repository)
        {
            Repository = repository;
        }

        // GET: api/Book
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Book>>> Get()
        {
            var result = await Repository.FindManyAsync();
            return Ok(result);
        }

        // GET: api/Book/5
        [HttpGet("{id}", Name = nameof(Get))]
        public async Task<ActionResult<Book>> Get(string id)
        {
            var result = await Repository.FindOneAsync(e => e.Id == id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        // POST: api/Book
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] Book value)
        {
            var result = await Repository.InsertOneAsync(value);
            return CreatedAtAction(nameof(Get), new { id = value.Id }, result);
        }

        // PUT: api/Book/5
        [HttpPut("{id}")]
        public async Task<ActionResult<Book>> Put(string id, [FromBody] Book value)
        {
            if (string.Compare(id, value.Id, true) != 0) return BadRequest();
            var result = await Repository.FindOneAndReplaceAsync(e => e.Id == id, value);
            return Ok(result);
        }

        // DELETE: api/Book/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(string id)
        {
            var count = await Repository.DeleteOneAsync(e => e.Id == id);
            if (count == 0) return NotFound();
            return NoContent();
        }
    }
}