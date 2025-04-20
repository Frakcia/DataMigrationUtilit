using DataMigrationUtilit.Application.Models.Employee;
using DataMigrationUtilit.Core.Entities;

namespace DataMigrationUtilit.Application.Mapping
{
    public class EmployeeMapping
    {
        public static void MapToEntity(EmployeeImportDto source, Employee destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            destination.FullName = source.FullName;
            destination.Login = source.Login;
            destination.Password = source.Password;
            destination.DepartmentName = source.DepartmentName;
            destination.JobTitleName = source.JobTitleName;
        }
    }
}
