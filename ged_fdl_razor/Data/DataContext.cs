using Microsoft.EntityFrameworkCore;
using ged_fdl_razor.Models;
using System;

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

            // Seed Admin par défaut
            modelBuilder.Entity<Commune>().HasData(
                new Commune
                {
                    CommuneID = 1,
                    Nom = "Admin",
                    Email = "admin@fdl.mg",
                    PasswordHash = "admin", // pour tests seulement, à hasher en production
                    Role = "Admin",
                    District = "Central",
                    Region = "Analamanga",
                    MustChangePassword = true,
                    IsActive = true,
                    DateCreation = DateTime.UtcNow
                }
            );
        }
    }
}
