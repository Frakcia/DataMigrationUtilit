using DataMigrationUtilit.Application.Contracts;
using DataMigrationUtilit.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationUtilit.Application.Serivces
{
    public class RelationBuilderService : IRelationBuilderService
    {
        private readonly AppDbContext _context;

        const int BatchSize = 1000;

        public RelationBuilderService(AppDbContext context)
        {
            _context = context;
        }

        public async Task BuildDepartmentRelations()
        {
            var skip = 0;

            while (true)
            {
                var departments = await _context.Departments
                    .Where(e=> !e.ManagerId.HasValue || !e.ParentId.HasValue)
                    .OrderBy(e => e.Id)
                    .Skip(skip)
                    .Take(BatchSize)
                    .ToListAsync();

                if (departments.Count == 0)
                {
                    break;
                }

                var parentNames = departments.Select(e => e.ParentName);
                var mangerNames = departments.Select(e => e.ManagerName);

                var existParentDepartments = await _context.Departments.Where(e => parentNames.Contains(e.Name)).ToListAsync();
                var existManagers = await _context.Employees.Where(e => mangerNames.Contains(e.FullName)).ToListAsync();

                if (existParentDepartments.Count > 0 || existManagers.Count > 0)
                {
                    foreach (var department in departments)
                    {
                        if (!department.ParentId.HasValue)
                        {
                            var parents = existParentDepartments.Where(e => e.Name == department.ParentName).ToArray();
                            if(parents.Length == 1)
                            {
                                department.ParentId = parents[0].Id;
                            }
                        }

                        if (!department.ManagerId.HasValue)
                        {
                            var manager = existManagers.FirstOrDefault(e=> e.FullName == department.ManagerName);
                            if (manager is not null)
                            {
                                department.ManagerId = manager.Id;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                skip += BatchSize;
            }
        }

        public async Task BuildEmployeeRelations()
        {
            var skip = 0;

            while (true)
            {
                var employees = await _context.Employees
                    .Where(e => !e.JobTitleId.HasValue || !e.DepartmentId.HasValue)
                    .OrderBy(e => e.Id)
                    .Skip(skip)
                    .Take(BatchSize)
                    .ToListAsync();

                if(employees.Count == 0)
                {
                    break;
                }

                var employeesJobNames = employees.Select(e => e.JobTitleName);
                var employeesDepartmentNames = employees.Select(e => e.DepartmentName);

                var existJobs = await _context.JobTitles
                    .Where(e=> employeesJobNames.Contains(e.Name))
                    .ToListAsync();

                var existDepartments = await _context.Departments
                    .Where(e => employeesDepartmentNames.Contains(e.Name))
                    .ToListAsync();

                if (existJobs.Count > 0 || existDepartments.Count > 0)
                {
                    foreach (var employee in employees)
                    {
                        if (!employee.JobTitleId.HasValue)
                        {
                            var job = existJobs.FirstOrDefault(e=> e.Name == employee.JobTitleName);
                            if (job is not null)
                            {
                                employee.JobTitleId = job.Id;
                            }
                        }

                        if(!employee.DepartmentId.HasValue)
                        {
                            var departments = existDepartments.Where(e=> e.Name ==employee.DepartmentName).ToArray();
                            if (departments.Length == 1)
                            {
                                employee.DepartmentId = departments[0].Id;
                            }
                        }
                    }

                    await _context.SaveChangesAsync();
                }

                skip += BatchSize;
            }
        }
    }
}
