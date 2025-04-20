using DataMigrationUtilit.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataMigrationUtilit.Infrastructure.EntityConfigurations
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.HasOne(e => e.Manager).WithMany();
            //builder.HasOne(e => e.Parent).WithMany();
            builder.HasIndex(e => e.ParentId);
        }
    }
}
