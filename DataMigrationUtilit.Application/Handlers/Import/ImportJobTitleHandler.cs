using DataMigrationUtilit.Application.Contracts;
using DataMigrationUtilit.Application.Mapping;
using DataMigrationUtilit.Application.Models.JobTitle;
using DataMigrationUtilit.Core.Entities;
using DataMigrationUtilit.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataMigrationUtilit.Application.Handlers.Import
{
    public class ImportJobTitleHandler : IDataImport
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ImportJobTitleHandler> _logger;

        public ImportJobTitleHandler(AppDbContext context, ILogger<ImportJobTitleHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Import(IEnumerable<string> lines, int minRowCount)
        {
            var importedJobTitles = ConvertLineToModels(lines, minRowCount);
            var importedName = importedJobTitles.Select(e => e.Name).ToArray();
            var existedJobs = await _context.JobTitles
                .Where(e => importedName.Contains(e.Name))
                .Select(e => e.Name)
                .ToHashSetAsync();

            var newJobTitles = new List<JobTitle>();
            foreach (var jobTitle in importedJobTitles)
            {
                if (existedJobs.Contains(jobTitle.Name))
                {
                    continue;
                }

                var newJobTitle = new JobTitle();

                JobTitleMapping.MapToEntity(jobTitle, newJobTitle);
                newJobTitles.Add(newJobTitle);
            }

            if (newJobTitles.Any())
            {
                await _context.AddRangeAsync(newJobTitles);
                await _context.SaveChangesAsync();
            }
        }

        private List<JobTitleImportDto> ConvertLineToModels(IEnumerable<string> lines, int minRowCount)
        {
            var currentRowCount = minRowCount;

            var importedEmployees = new List<JobTitleImportDto>();
            foreach (var line in lines)
            {
                try
                {
                    var lineColums = line.Split('\t');
                    importedEmployees.Add(new JobTitleImportDto(lineColums[0]));
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Ошибка в строке {currentRowCount}, описание ошибки: {ex.Message}");
                }
                currentRowCount++;
            }

            return importedEmployees;
        }
    }
}
