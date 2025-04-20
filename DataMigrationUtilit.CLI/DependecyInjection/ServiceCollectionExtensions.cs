using DataMigrationUtilit.Application.Contracts;
using DataMigrationUtilit.Application.Handlers.GetData;
using DataMigrationUtilit.Application.Handlers.Import;
using DataMigrationUtilit.Application.Serivces;
using DataMigrationUtilit.Infrastructure.Contexts;
using DataMigrationUtilit.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DataMigrationUtilit.CLI.DependecyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddConsoleLogging(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddLogging(configure => configure.AddConsole());
        }
        public static IServiceCollection AddDbContext(this IServiceCollection serviceCollection, HostBuilderContext context)
        {
            return serviceCollection
                .AddDbContext<AppDbContext>(options => 
                options.UseNpgsql(context.Configuration.GetConnectionString("DefaultConnection")));
        }

        public static IServiceCollection RegisterServices(this IServiceCollection serviceCollection)
        {
            return serviceCollection
                .AddScoped<IFileService, FileService>()
                .AddScoped<IImportDataService, ImportDataService>()
                .AddScoped<IRelationBuilderService, RelationBuilderService>()
                .AddScoped<DbService>()
                .AddScoped<GetDataHandler>()
                .AddScoped<ImportJobTitleHandler>()
                .AddScoped<ImprotDepartmentsHandler>()
                .AddScoped<ImportEmployeesHandler>();
        }
    }
}
