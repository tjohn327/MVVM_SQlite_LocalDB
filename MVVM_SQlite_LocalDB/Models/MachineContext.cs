using Microsoft.EntityFrameworkCore;

namespace MVVM_SQlite_LocalDB.Models
{
    public class MachineContext : DbContext
    {
        public DbSet<Machine> Machines { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder options)
                => options.UseSqlite("Data Source=LocalDB.db");
    }
}