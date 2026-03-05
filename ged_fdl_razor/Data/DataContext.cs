using Microsoft.EntityFrameworkCore;
using ged_fdl_razor.Models;

namespace ged_fdl_razor.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        public DbSet<Commune> Communes { get; set; }
        public DbSet<DossierFinancement> DossiersFinancement { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Remarque> Remarques { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

        }
    }
}