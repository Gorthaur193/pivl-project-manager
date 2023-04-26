using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using project_managet_models;
using project_managet_models.Models;

namespace project_managet_dblayer
{
    public class ProjectManagerContext : DbContext
    {
        private const string configPath = "db_config.json";

        public DbSet<Device> Devices { get; set; }  
        public DbSet<Project> Projects { get; set; }
        public DbSet<JobTitle> JobTitles { get; set; }
        public DbSet<Employee> Employees { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!File.Exists(configPath))
                throw new FileNotFoundException("Missing db_config.");

            optionsBuilder
                .UseLazyLoadingProxies()
                .UseSqlServer(JObject.Parse(
                    File.ReadAllText(configPath))["connectionString"]?
                .ToString() 
                ?? throw new KeyNotFoundException("connectionString is missing."));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            foreach (var type in typeof(IEntity)
                                    .Assembly
                                    .GetTypes()
                                    .Where(x => x.IsAssignableTo(typeof(IEntity)) && x.IsClass))
                modelBuilder.Entity(type)
                    .Property(nameof(IEntity.Id))
                    .HasDefaultValueSql("NEWSEQUENTIALID()");

            modelBuilder.Entity<Employee>()
                .HasOne(x => x.Supervisor)
                .WithMany(x => x.Supervisees)
                .OnDelete(DeleteBehavior.SetNull)
                .OnDelete(DeleteBehavior.ClientSetNull);
        }
    }
}