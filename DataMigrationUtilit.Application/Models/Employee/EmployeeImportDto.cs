using DataMigrationUtilit.Application.Extensions;

namespace DataMigrationUtilit.Application.Models.Employee
{
    public class EmployeeImportDto
    {
        public EmployeeImportDto(string fullName, string login, string password, string? departmentName, string? jobTitleName)
        {
            FullName = fullName.FixFullNameFormat();
            Login = login.FixFormat();
            Password = password.FixFormat();
            if (!string.IsNullOrWhiteSpace(departmentName))
            {
                DepartmentName = departmentName.FixFormat();
            }
            if (!string.IsNullOrWhiteSpace(jobTitleName))
            {
                JobTitleName = jobTitleName.FixFormat();
            }
        }

        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }
        public string? DepartmentName { get; set; }
        public string? JobTitleName { get; set; }
    }
}
