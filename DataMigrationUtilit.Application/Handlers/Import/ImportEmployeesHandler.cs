using DataMigrationUtilit.Application.Contracts;
using DataMigrationUtilit.Application.Mapping;
using DataMigrationUtilit.Application.Models.Employee;
using DataMigrationUtilit.Core.Entities;
using DataMigrationUtilit.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataMigrationUtilit.Application.Handlers.Import
{
    public class ImportEmployeesHandler : IDataImport
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ImportEmployeesHandler> _logger;

        public ImportEmployeesHandler(AppDbContext context, ILogger<ImportEmployeesHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Import(IEnumerable<string> lines, int minRowCount)
        {
            var importedEmployees = ConvertLineToModels(lines, minRowCount);
            var importedEmployeesFullNames = importedEmployees.Select(e => e.FullName).ToArray();

            var existedEmployees = await _context.Employees
                .Where(e => importedEmployeesFullNames.Contains(e.FullName))
                .ToListAsync();

            var newEmployees = new List<Employee>();
            foreach (var employee in importedEmployees)
            {
                Employee? currentEmployee = existedEmployees.FirstOrDefault(e => e.FullName == employee.FullName);

                if (currentEmployee is null)
                {
                    currentEmployee = new Employee();
                }

                var oldDepartmentName = currentEmployee.DepartmentName;
                var oldJobTitleName = currentEmployee.JobTitleName;

                EmployeeMapping.MapToEntity(employee, currentEmployee);

                if (currentEmployee.Id > 0)
                {
                    if (currentEmployee.DepartmentId.HasValue && currentEmployee.DepartmentName != oldDepartmentName)
                    {
                        currentEmployee.DepartmentId = null;
                    }

                    if (currentEmployee.JobTitleId.HasValue && currentEmployee.JobTitleName != oldJobTitleName)
                    {
                        currentEmployee.JobTitleId = null;
                    }
                }else
                {
                    newEmployees.Add(currentEmployee);
                }
            }

            if (newEmployees.Any())
            {
                await _context.Employees.AddRangeAsync(newEmployees);
            }

            await _context.SaveChangesAsync();
        }

        private List<EmployeeImportDto> ConvertLineToModels(IEnumerable<string> lines, int minRowCount)
        {
            var currentRowCount = minRowCount;

            var importedEmployees = new List<EmployeeImportDto>();
            foreach (var line in lines)
            {
                try
                {
                    var lineColums = line.Split('\t');

                    if(string.IsNullOrWhiteSpace(lineColums[0]))
                    {
                        _logger.LogError($"Ошибка в строке {currentRowCount}, описание ошибки: не указано подразделение");
                        continue;
                    }
                    if (string.IsNullOrWhiteSpace(lineColums[4]))
                    {
                        _logger.LogError($"Ошибка в строке {currentRowCount}, описание ошибки: не указана должность");
                        continue;
                    }

                    importedEmployees.Add(new EmployeeImportDto(lineColums[1], lineColums[2], lineColums[3], lineColums[0], lineColums[4]));
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
