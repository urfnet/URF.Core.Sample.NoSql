# URF Core NoSQL Demo

Sample Unit of Work and Repository Framework (URF) with MongoDb

## Prerequisites

- Install [Docker Desktop](https://www.docker.com/products/docker-desktop).
- Install [Robo 3T](https://robomongo.org/download).
- Install [.NET Core SDK](https://dotnet.microsoft.com/download) 6.0 or greater.

## Local MongoDb with Docker

- Run MongoDb Version 3.x via Docker.
  - This command will mount a volume where a shared database will reside on the host system in /tmp (MacOS).
  > **Note**: Data will persist even if you remove and re-create the container.
    ```
    docker run --name mongo -d -p 27017:27017 -v /tmp/mongo/data:/data/db mongo
    ```
  - This command will mount a volume where a shared database will reside in the container (Windows).
  > **Note**: MongoDB Docker image has a bug which prevents mounting a host volume in Windows.

  > **Note**: Data will persist if you start and stop the container.
  > You will lose data if you remove the container.
    ```
    docker run --name mongo -d -p 27017:27017 -v /data/db mongo
    ```
  - Check to see if it is running.
    ```
    docker ps
    ```
- Connect to the mongo shell.
    ```
    docker exec -it mongo bash
    mongo
    ```
- Create the database, add a collection, insert data.
    ```
    use BookstoreDb
    db.createCollection("Books")
    db.Books.insertMany([{"Name":"Design Patterns","Price":54.93,"Category":"Computers","Author":"Ralph Johnson"},{"Name":"Clean Code","Price":43.15,"Category":"Computers","Author":"Robert C. Martin"}])
    db.createCollection("Authors")
    db.Authors.insertMany([{"Name":"Ralph Johnson","Country":"United States"},{"Name":"Robert C. Martin","Country":"United Kingdom"}])
    ```
- View the documents in the database.
    ```
    db.Books.find({}).pretty()
    db.Authors.find({}).pretty()
    ```
- Connect to MongoDb from Robo 3T on port 27017.
    ![robo-3t-win-local-connection](images/robo-3t-win-local-connection.png)

## Models, Abstractions, Mongo Projects

1. Create a `Models` .NET class library.
   - Add package: MongoDB.Bson
   - Add a `Book` model to the Models folder.
    ```csharp
    public class Book
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        [BsonElement("Name")]
        public string BookName { get; set; }

        public decimal Price { get; set; }

        public string Category { get; set; }

        public string Author { get; set; }
    }
    ```

   - Add an `Author` model to the Models folder.
    ```csharp
    public class Author
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }

        public string Name { get; set; }

        public string Country { get; set; }
    }
    ```

2. Create an `Abstractions` .NET class library.
   - Add package: URF.Core.Abstractions
   - Reference the `Models` project.
   - Add `IUrfSampleUnitOfWork` interface with properties for authors and books repositories.

    ```csharp
    public interface IBookstoreUnitOfWork
    {
        public IDocumentRepository<Author> AuthorsRepository { get; }

        public IDocumentRepository<Book> BooksRepository { get; }
    }
    ```

3. Create an `Mongo` .NET class library.
   - Add package: URF.Core.Mongo
   - Reference the `Models` and `Abstractions` projects.
   - Add `UrfSampleUnitOfWork` interface with properties for authors and books repositories.

    ```csharp
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
    ```

## Web API Project

1. Add `Api` ASP.NET Core Web API project.
   - Remove `WeatherForecast` and `WeatherForecastController` classes.
   - Add NuGet package: URF.Core.Mongo
   - Reference Models, Abstractions and Mongo projects.
   - Add `BookstoreDatabaseSettings` section to **appsettings.json**.
        ```json
        "BookstoreDatabaseSettings": {
          "BooksCollectionName": "Books",
          "ConnectionString": "mongodb://localhost:27017",
          "DatabaseName": "BookstoreDb"
        }
        ```
   - Add `BookstoreDatabaseSettings` class.
       ```csharp
       public class BookstoreDatabaseSettings : IBookstoreDatabaseSettings
       {
           public string AuthorsCollectionName { get; set; }
           public string BooksCollectionName { get; set; }
           public string ConnectionString { get; set; }
           public string DatabaseName { get; set; }
       }
       ```
2. Update `ConfigureServices` in `Startup` to register `IBookstoreDatabaseSettings` and `IMongoDatabase`.
   - Also register `IDocumentRepository<Author>`, `IDocumentRepository<Book>`, `IUrfSampleUnitOfWork`.
    ```csharp
    public void ConfigureServices(IServiceCollection services)
    {
        // Register settings
        services.Configure<BookstoreDatabaseSettings>(
            Configuration.GetSection(nameof(BookstoreDatabaseSettings)));
        services.AddSingleton<IBookstoreDatabaseSettings>(sp =>
            sp.GetRequiredService<IOptions<BookstoreDatabaseSettings>>().Value);

        // Register Mongo client and collections
        services.AddSingleton<IMongoDatabase>(sp =>
        {
            var settings = sp.GetRequiredService<IBookstoreDatabaseSettings>();
            var client = new MongoClient(settings.ConnectionString);
            return client.GetDatabase(settings.DatabaseName);
        });
        services.AddSingleton<IMongoCollection<Author>>(sp =>
        {
            var context = sp.GetRequiredService<IMongoDatabase>();
            var settings = sp.GetRequiredService<IBookstoreDatabaseSettings>();
            return context.GetCollection<Author>(settings.AuthorsCollectionName);
        });
        services.AddSingleton<IMongoCollection<Book>>(sp =>
        {
            var context = sp.GetRequiredService<IMongoDatabase>();
            var settings = sp.GetRequiredService<IBookstoreDatabaseSettings>();
            return context.GetCollection<Book>(settings.BooksCollectionName);
        });

        // Register unit of work and repositories
        services.AddSingleton<IDocumentRepository<Author>, DocumentRepository<Author>>();
        services.AddSingleton<IDocumentRepository<Book>, DocumentRepository<Book>>();
        services.AddSingleton<IBookstoreUnitOfWork, BookstoreUnitOfWork>();

        // Register controllers
        services.AddControllers();
    }
    ```

## Model Controller

1. Add `Book` Web API controller to the `Controllers` folder.
    ```
    dotnet aspnet-codegenerator controller -name BookController -actions -api -outDir Controllers
    ```
2. Inject `IUrfSampleUnitOfWork` into the controller ctor.
    ```csharp
    public IBookstoreUnitOfWork UnitOfWork { get; }

    public BookController(IBookstoreUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }
    ```
3. Add methods for Get, Post, Put and Delete.
    ```csharp
    // GET: api/Book
    public async Task<ActionResult<IEnumerable<Book>>> Get()
    {
        var result = await UnitOfWork.BooksRepository
            .Queryable()
            .OrderBy(e => e.BookName)
            .ToListAsync();
        return Ok(result);
    }

    // GET: api/Book/5
    [HttpGet("{id}", Name = nameof(Get))]
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
    ```

## Web API Tests with Postman

1. Start the Web API project and test with Postman.
   - Replace `id` below with actual object id.

    ```
    GET: https://localhost:5100/api/book
    GET: https://localhost:5100/api/book/5e6d2f31d40521f24f7e582f
    ```
    ```
    POST: https://localhost:5100/api/book
    ```
    ```json
    {
        "bookName": "CLR via C#",
        "price": 49.99,
        "category": "Computers",
        "author": "Jeffrey Richter"
    }
    ```
    - Should return **201 Created** with correct Location response header.
    ```
    PUT: https://localhost:5100/api/book/5e6d47bfa70ee6419095d127
    ```
    ```json
    {
        "id": "5e6d47bfa70ee6419095d127", // Use generated id
        "bookName": "CLR via C#",
        "price": 59.99,
        "category": "Computers",
        "author": "Jeffrey Richter"
    }
    ```
    ```
    DELETE: https://localhost:5100/api/book/5e6d47bfa70ee6419095d127
    ```
    ```
    GET: https://localhost:5100/api/book/5e6d47bfa70ee6419095d127
    ```
    - Should return **404 Not Found**.

