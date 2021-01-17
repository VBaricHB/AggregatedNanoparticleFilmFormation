
using ANPaX.IO.DTO;

using Microsoft.EntityFrameworkCore;

namespace ANPaX.IO.DBConnection.Data
{
    public class DataContext : DbContext
    {
        public DbSet<AggregateConfigurationDTO> AggConfigs { get; set; }
        public DbSet<FilmFormationConfigurationDTO> FilmFormationConfigs { get; set; }
        public DbSet<ParticleSimulationDTO> ParticleSimulations { get; set; }
        public DbSet<PrimaryParticleDTO> PrimaryParticles { get; set; }
        public DbSet<UserDTO> Users { get; set; }
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AggregateConfigurationDTO>().ToTable("AggegrateConfiguration");
            modelBuilder.Entity<FilmFormationConfigurationDTO>().ToTable("FilmFormationConfiguration");
            modelBuilder.Entity<ParticleSimulationDTO>().ToTable("ParticleSimulationData");
            modelBuilder.Entity<PrimaryParticleDTO>().ToTable("PrimaryParticleData");
            modelBuilder.Entity<UserDTO>().ToTable("User");
        }
    }
}
