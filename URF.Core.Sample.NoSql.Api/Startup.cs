using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using URF.Core.Abstractions;
using URF.Core.Mongo;
using URF.Core.Sample.NoSql.Abstractions;
using URF.Core.Sample.NoSql.Api.Settings;
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
            services.AddControllers();
            services.Configure<BookstoreDatabaseSettings>(
                Configuration.GetSection(nameof(BookstoreDatabaseSettings)));
            services.AddSingleton<IBookstoreDatabaseSettings>(sp =>
                sp.GetRequiredService<IOptions<BookstoreDatabaseSettings>>().Value);
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
            services.AddSingleton<IDocumentRepository<Author>, DocumentRepository<Author>>();
            services.AddSingleton<IDocumentRepository<Book>, DocumentRepository<Book>>();
            services.AddSingleton<IBookstoreUnitOfWork, BookstoreUnitOfWork>();
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
