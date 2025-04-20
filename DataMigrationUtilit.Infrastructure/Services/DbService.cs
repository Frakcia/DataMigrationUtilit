using DataMigrationUtilit.Infrastructure.Contexts;
using Microsoft.EntityFrameworkCore;

namespace DataMigrationUtilit.Infrastructure.Services
{
    public class DbService
    {
        private readonly AppDbContext _context;

        public DbService(AppDbContext context)
        {
            _context = context;
        }

        public void ApplyMigrations()
        {
            _context.Database.Migrate();
        }
    }
}
