using Microsoft.Extensions.Options;
using MongoDB.Driver;
using URF.Core.Abstractions;
using URF.Core.Mongo;
using URF.Core.Sample.NoSql.Abstractions;
using URF.Core.Sample.NoSql.Api.Configuration;
using URF.Core.Sample.NoSql.Models;
using URF.Core.Sample.NoSql.Mongo;

namespace URF.Core.Sample.NoSql.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Print connection string
            var connectionString = Configuration["BookstoreDatabaseSettings:ConnectionString"];
            Console.WriteLine($"Connection String: {connectionString}");

            // Register settings
            services.Configure<BookstoreDatabaseSettings>(
                Configuration.GetSection(nameof(BookstoreDatabaseSettings)));
            services.AddSingleton(sp =>
                sp.GetRequiredService<IOptions<BookstoreDatabaseSettings>>().Value);

            // Register Mongo client and collections
            services.AddSingleton(sp =>
            {
                var settings = sp.GetRequiredService<BookstoreDatabaseSettings>();
                var client = new MongoClient(settings.ConnectionString);
                return client.GetDatabase(settings.DatabaseName);
            });
            services.AddSingleton(sp =>
            {
                var context = sp.GetRequiredService<IMongoDatabase>();
                var settings = sp.GetRequiredService<BookstoreDatabaseSettings>();
                return context.GetCollection<Author>(settings.AuthorsCollectionName);
            });
            services.AddSingleton(sp =>
            {
                var context = sp.GetRequiredService<IMongoDatabase>();
                var settings = sp.GetRequiredService<BookstoreDatabaseSettings>();
                return context.GetCollection<Book>(settings.BooksCollectionName);
            });

            // Register unit of work and repositories
            services.AddSingleton<IDocumentRepository<Author>, DocumentRepository<Author>>();
            services.AddSingleton<IDocumentRepository<Book>, DocumentRepository<Book>>();
            services.AddSingleton<IBookstoreUnitOfWork, BookstoreUnitOfWork>();

            // Register controllers
            services.AddControllers();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
