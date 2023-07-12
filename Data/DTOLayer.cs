using Microsoft.EntityFrameworkCore;
using Searching.Models.Domain;

namespace Searching.Data
{
    public class DTOLayer:DbContext
    {
        public DTOLayer(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Student> Students { get; set; }
    }
}
