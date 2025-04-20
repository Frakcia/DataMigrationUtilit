namespace DataMigrationUtilit.Core.Entities
{
    public class Employee
    {
        public long Id { get; set; }
        public string FullName { get; set; }
        public string Login { get; set; }
        public string Password { get; set; }

        public long? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
       // public bool IsRequiredDepartment { get; set; }

        public long? JobTitleId { get; set; }
        public JobTitle? JobTitle { get; set; }
        public string JobTitleName { get; set; } = string.Empty;
        //public bool IsRequiredJobTitle { get; set; }
    }
}
