using DataMigrationUtilit.Application.Contracts;
using DataMigrationUtilit.Application.Handlers.Import;
using DataMigrationUtilit.Infrastructure.Contexts;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace DataMigrationUtilit.Application.Serivces
{
    public class ImportDataService : IImportDataService
    {
        private readonly AppDbContext _context;
        private readonly IFileService _fileService;
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ImportDataService> _logger;
        private readonly IRelationBuilderService _relationBuilderService;

        const int BatchCount = 1000;

        public ImportDataService(AppDbContext context, IFileService fileService,
            IServiceProvider serviceProvider, ILogger<ImportDataService> logger, IRelationBuilderService relationBuilderService)
        {
            _context = context;
            _fileService = fileService;
            _serviceProvider = serviceProvider;
            _logger = logger;
            _relationBuilderService = relationBuilderService;
        }


        public async Task Import(string fileName, string type)
        {
            try
            {
                var handler = GetImportService(type);

                if(handler == null)
                {
                    _logger.LogError("Операция завершена, указаный тип не соответсвует ожидаемым");
                    return;
                }

                if(!_fileService.IsFileExist(fileName))
                {
                    _logger.LogError("Файл не найден");
                    return;
                }

                var rowCount = 1;
                await foreach (var lines in _fileService.ReadLinesAsync(fileName, BatchCount))
                {
                    await handler.Import(lines, rowCount);
                    rowCount += BatchCount;
                }

                await _relationBuilderService.BuildDepartmentRelations();
                await _relationBuilderService.BuildEmployeeRelations();

            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);
            }
        }

        public IDataImport? GetImportService(string type)
        {
            if (type == "подразделение")
            {
                var departmentHandler = _serviceProvider.GetRequiredService<ImprotDepartmentsHandler>();
                return departmentHandler;
            }
            else if (type == "сотрудник")
            {
                var departmentHandler = _serviceProvider.GetRequiredService<ImportEmployeesHandler>();
                return departmentHandler;
            }
            else if (type == "должность")
            {
                var departmentHandler = _serviceProvider.GetRequiredService<ImportJobTitleHandler>();
                return departmentHandler;
            }

            return null;
        }
    }
}
