namespace DataMigrationUtilit.Core.Entities
{
    public class Department
    {
        public long Id { get; set; }

        public long? ParentId { get; set; }
        public string? ParentName { get; set; }

        public long? ManagerId { get; set; }
        public Employee? Manager { get; set; }
        public string ManagerName { get; set; }

        public string Name { get; set; }
        public string Phone { get; set; }

        public IEnumerable<Employee> Employees { get; set; }
    }
}
