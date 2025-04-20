using DataMigrationUtilit.Application.Models.Department;
using DataMigrationUtilit.Core.Entities;

namespace DataMigrationUtilit.Application.Mapping
{
    public class DepartmentMapping
    {
        public static void MapToEntity(DepartmentImportDto source, Department destination)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (destination == null)
            {
                throw new ArgumentNullException(nameof(destination));
            }

            destination.Name = source.Name;
            destination.Phone = source.Phone;
            destination.ParentName = source.ParentName;
            destination.ManagerName = source.ManagerName;
        }
    }
}
