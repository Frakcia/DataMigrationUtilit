using DataMigrationUtilit.Application.Contracts;
using DataMigrationUtilit.Application.Mapping;
using DataMigrationUtilit.Application.Models.Department;
using DataMigrationUtilit.Core.Entities;
using DataMigrationUtilit.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DataMigrationUtilit.Application.Handlers.Import
{
    public class ImprotDepartmentsHandler : IDataImport
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ImprotDepartmentsHandler> _logger;

        public ImprotDepartmentsHandler(AppDbContext context, ILogger<ImprotDepartmentsHandler> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task Import(IEnumerable<string> lines, int minRowCount)
        {
            var importedDepartments = ConvertLineToModels(lines, minRowCount);
            var importedDepartmentsNames = importedDepartments.Select(e => e.Name).ToArray();

            var existedDepartments = await _context.Departments
                .Where(e => importedDepartmentsNames.Contains(e.Name))
                .ToListAsync();

            existedDepartments = existedDepartments
                .Where(e => importedDepartments.Any(x => e.Name == x.Name && (string.IsNullOrWhiteSpace(e.ParentName) ? true : e.ParentName == x.ParentName)))
                .ToList();

            var newDepartments = new List<Department>();
            foreach (var department in importedDepartments)
            {
                Department? currentDepartment = existedDepartments.FirstOrDefault(e => e.Name == department.Name && e.ParentName == department.ParentName);

                if (currentDepartment is null)
                {
                    currentDepartment = new Department();
                }

                var oldParentName = currentDepartment.ParentName;
                var oldManagerName = currentDepartment.ManagerName;

                DepartmentMapping.MapToEntity(department, currentDepartment);

                if(string.IsNullOrWhiteSpace(currentDepartment.ParentName))
                {
                    currentDepartment.ParentId = 0;
                }

                if(currentDepartment.Id > 0)
                {
                    if (currentDepartment.ParentId.HasValue && currentDepartment.ParentName != oldParentName)
                    {
                        currentDepartment.ParentId = null;
                    }

                    if (currentDepartment.ManagerId.HasValue && currentDepartment.ManagerName != oldManagerName)
                    {
                        currentDepartment.ManagerId = null;
                    }

                } else
                {
                    newDepartments.Add(currentDepartment);
                }
            }

            if (newDepartments.Any())
            {
                await _context.Departments.AddRangeAsync(newDepartments);
            }

            await _context.SaveChangesAsync();
        }

        private List<DepartmentImportDto> ConvertLineToModels(IEnumerable<string> lines, int minRowCount)
        {
            var currentRowCount = minRowCount;

            var importedEmployees = new List<DepartmentImportDto>();
            foreach (var line in lines)
            {
                try
                {
                    var lineColums = line.Split('\t');

                    if (string.IsNullOrWhiteSpace(lineColums[0]))
                    {
                        _logger.LogError($"Ошибка в строке {currentRowCount}, описание ошибки: не указано название подразделения");
                        continue;
                    } 
                    
                    if (string.IsNullOrWhiteSpace(lineColums[2]))
                    {
                        _logger.LogError($"Ошибка в строке {currentRowCount}, описание ошибки: не указан управляющий подразделения");
                        continue;
                    }

                    importedEmployees.Add(new DepartmentImportDto(lineColums[0], lineColums[3], lineColums[1], lineColums[2]));
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
