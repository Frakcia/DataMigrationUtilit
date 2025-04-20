using DataMigrationUtilit.Application.Extensions;

namespace DataMigrationUtilit.Application.Models.Department
{
    public class DepartmentImportDto
    {
        public DepartmentImportDto(string name, string phone, string parentName, string managerName)
        {
            Name = name.FixFormat();
            Phone = phone.FixFormat();
            if (!string.IsNullOrWhiteSpace(parentName))
            {
                ParentName = parentName.FixFormat();
            }
            ManagerName = managerName.FixFullNameFormat();
        }

        public string Name { get; set; }
        public string Phone { get; set; }
        public string? ParentName { get; set; }
        public string ManagerName { get; set; }
    }
}
