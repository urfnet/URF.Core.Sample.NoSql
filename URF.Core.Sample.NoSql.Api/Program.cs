using Microsoft.Extensions.Options;
using MongoDB.Driver;
using URF.Core.Abstractions;
using URF.Core.Mongo;
using URF.Core.Sample.NoSql.Abstractions;
using URF.Core.Sample.NoSql.Api.Configuration;
using URF.Core.Sample.NoSql.Models;
using URF.Core.Sample.NoSql.Mongo;

var builder = WebApplication.CreateBuilder(args);

// Add builder.Services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Print connection string
var connectionString = builder.Configuration["BookstoreDatabaseSettings:ConnectionString"];
Console.WriteLine($"Connection String: {connectionString}");

// Register settings
builder.Services.Configure<BookstoreDatabaseSettings>(
    builder.Configuration.GetSection(nameof(BookstoreDatabaseSettings)));
builder.Services.AddSingleton(sp =>
    sp.GetRequiredService<IOptions<BookstoreDatabaseSettings>>().Value);

// Register Mongo client and collections
builder.Services.AddSingleton(sp =>
{
    var settings = sp.GetRequiredService<BookstoreDatabaseSettings>();
    var client = new MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.DatabaseName);
});
builder.Services.AddSingleton(sp =>
{
    var context = sp.GetRequiredService<IMongoDatabase>();
    var settings = sp.GetRequiredService<BookstoreDatabaseSettings>();
    return context.GetCollection<Author>(settings.AuthorsCollectionName);
});
builder.Services.AddSingleton(sp =>
{
    var context = sp.GetRequiredService<IMongoDatabase>();
    var settings = sp.GetRequiredService<BookstoreDatabaseSettings>();
    return context.GetCollection<Book>(settings.BooksCollectionName);
});

// Register unit of work and repositories
builder.Services.AddSingleton<IDocumentRepository<Author>, DocumentRepository<Author>>();
builder.Services.AddSingleton<IDocumentRepository<Book>, DocumentRepository<Book>>();
builder.Services.AddSingleton<IBookRepository, BookRepository>();
builder.Services.AddSingleton<IBookstoreUnitOfWork, BookstoreUnitOfWork>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
