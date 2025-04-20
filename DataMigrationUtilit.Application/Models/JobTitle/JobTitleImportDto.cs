using DataMigrationUtilit.Application.Extensions;

namespace DataMigrationUtilit.Application.Models.JobTitle
{
    public class JobTitleImportDto
    {
        public JobTitleImportDto(string name)
        {
            Name = name.FixFormat();
        }

        public string Name { get; set; }
    }
}
