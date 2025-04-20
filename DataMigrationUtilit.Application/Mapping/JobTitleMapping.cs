using DataMigrationUtilit.Application.Models.JobTitle;
using DataMigrationUtilit.Core.Entities;

namespace DataMigrationUtilit.Application.Mapping
{
    public class JobTitleMapping
    {
        public static void MapToEntity(JobTitleImportDto source, JobTitle destination)
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
        }
    }
}
