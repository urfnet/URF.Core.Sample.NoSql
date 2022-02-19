using Microsoft.AspNetCore.Mvc;
using URF.Core.Mongo;
using URF.Core.Sample.NoSql.Abstractions;
using URF.Core.Sample.NoSql.Models;

namespace URF.Core.Sample.NoSql.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        public IBookstoreUnitOfWork UnitOfWork { get; }

        public AuthorController(IBookstoreUnitOfWork unitOfWork)
        {
            UnitOfWork = unitOfWork;
        }

        // GET: api/Author
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> Get()
        {
            var result = await UnitOfWork.AuthorsRepository
                .Queryable()
                .OrderBy(e => e.Name)
                .ToListAsync();
            return Ok(result);
        }

        // GET: api/Author/5
        [HttpGet("{id:guid}")]
        public async Task<ActionResult<Author>> Get(Guid id)
        {
            var result = await UnitOfWork.AuthorsRepository.FindOneAsync(e => e.Id == id);
            if (result == null) return NotFound();
            return Ok(result);
        }


        // POST: api/Author
        [HttpPost]
        public async Task<ActionResult<Author>> Post([FromBody] Author value)
        {
            var result = await UnitOfWork.AuthorsRepository.InsertOneAsync(value);
            return CreatedAtAction(nameof(Get), new { id = value.Id }, result);
        }

        // PUT: api/Author/5
        [HttpPut("{id:guid}")]
        public async Task<ActionResult<Author>> Put(Guid id, [FromBody] Author value)
        {
            if (id != value.Id) return BadRequest();
            var result = await UnitOfWork.AuthorsRepository.FindOneAndReplaceAsync(e => e.Id == id, value);
            return Ok(result);
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var count = await UnitOfWork.AuthorsRepository.DeleteOneAsync(e => e.Id == id);
            if (count == 0) return NotFound();
            return NoContent();
        }
    }
}
