using DataMigrationUtilit.Core.Entities;
using DataMigrationUtilit.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationUtilit.Application.Handlers.GetData
{
    public class DepartmentOutDto
    {
        public DepartmentOutDto(long id, string name, long? managerId, IEnumerable<EmployeeOutDto> employees)
        {
            Id = id;
            Name = name;
            ManagerId = managerId;
            Employees = employees;
        }
        public long Id { get; set; }
        public string Name { get; set; }
        public long? ManagerId { get; set; }
        public IEnumerable<EmployeeOutDto> Employees { get; set; } = Enumerable.Empty<EmployeeOutDto>();
        public IEnumerable<DepartmentOutDto> Departments { get; set; } = Enumerable.Empty<DepartmentOutDto>();
    }
    public class EmployeeOutDto
    {
        public EmployeeOutDto(long id, string fullName, long? departmentId, long jobTitleId, string jobTitleName)
        {
            Id = id;
            FullName = fullName;
            DepartmentId = departmentId;
            JobTitleId = jobTitleId;
            JobTitleName = jobTitleName;
        }
        public long Id { get; set; }
        public long? DepartmentId { get; set; }
        public string FullName { get; set; }
        public long JobTitleId { get; set; }
        public string JobTitleName { get; set; }
    }

    public class GetDataHandler
    {
        private readonly AppDbContext _context;

        public GetDataHandler(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<DepartmentOutDto>> GetData(long? departmentId)
        {
            var query = _context.Departments;

            var roots = departmentId.HasValue 
                ? query.Where(e => e.ParentId.HasValue && e.ParentId.Value == departmentId.Value).ToList()
                : query.Where(e=> e.ParentId.HasValue && e.ParentId.Value == 0).ToList();

            IEnumerable<DepartmentOutDto> result = await GetDepartmentHierarchy(roots);

            return result;
        }

        private async Task<IEnumerable<DepartmentOutDto>> GetDepartmentHierarchy(IEnumerable<Department> departments)
        {
            List<DepartmentOutDto> result = new List<DepartmentOutDto>();
            var departmentIds = departments.Select(e=> e.Id).ToArray();
            var allEmployees = _context.Employees
                .Include(e=> e.JobTitle)
                .Where(e => e.JobTitleId.HasValue
                         && e.DepartmentId.HasValue
                         && departmentIds.Contains(e.DepartmentId.Value))
                .Select(e=> new EmployeeOutDto(e.Id, e.FullName, e.DepartmentId, e.JobTitleId.Value, e.JobTitle.Name))
                .ToList();

            foreach (var department in departments)
            {
                var employees = allEmployees.Where(e => e.DepartmentId.Value == department.Id).ToArray();
                var dto = new DepartmentOutDto(department.Id, department.Name, department.ManagerId, employees);

                result.Add(dto);

                var childrenDepartments = _context.Departments.Where(e=> e.ParentId.HasValue && e.ParentId.Value == department.Id).ToList();

                if (childrenDepartments.Any())
                {
                    dto.Departments = await GetDepartmentHierarchy(childrenDepartments);
                }
            }

            return result;
        }
    }
}
