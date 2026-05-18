using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace WorldApp.Models;

public class WorldContext : IdentityDbContext
{
    public WorldContext(DbContextOptions<WorldContext> options) : base(options) { }

    public DbSet<Country> Countries { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<CountryLanguage> CountryLanguages { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Mapeo de la tabla Country (la tabla se llama "country" en la BD)
        modelBuilder.Entity<Country>(entity =>
        {
            entity.ToTable("country");
            entity.HasKey(c => c.Code);
            entity.Property(c => c.Code).HasColumnName("Code").HasMaxLength(3);
            entity.Property(c => c.Name).HasColumnName("Name").HasMaxLength(52);
            entity.Property(c => c.Continent).HasColumnName("Continent");
            entity.Property(c => c.Region).HasColumnName("Region").HasMaxLength(26);
            entity.Property(c => c.SurfaceArea).HasColumnName("SurfaceArea").HasColumnType("decimal(10,2)");
            entity.Property(c => c.IndepYear).HasColumnName("IndepYear");
            entity.Property(c => c.Population).HasColumnName("Population");
            entity.Property(c => c.LifeExpectancy).HasColumnName("LifeExpectancy").HasColumnType("decimal(3,1)");
            entity.Property(c => c.GNP).HasColumnName("GNP").HasColumnType("decimal(10,2)");
            entity.Property(c => c.GNPOld).HasColumnName("GNPOld").HasColumnType("decimal(10,2)");
            entity.Property(c => c.LocalName).HasColumnName("LocalName").HasMaxLength(45);
            entity.Property(c => c.GovernmentForm).HasColumnName("GovernmentForm").HasMaxLength(45);
            entity.Property(c => c.HeadOfState).HasColumnName("HeadOfState").HasMaxLength(60);
            entity.Property(c => c.Capital).HasColumnName("Capital");
            entity.Property(c => c.Code2).HasColumnName("Code2").HasMaxLength(2);
            entity.HasMany(c => c.Cities).WithOne(c => c.Country).HasForeignKey(c => c.CountryCode);
            entity.HasMany(c => c.Languages).WithOne(l => l.Country).HasForeignKey(l => l.CountryCode);
        });

        // Mapeo de City
        modelBuilder.Entity<City>(entity =>
        {
            entity.ToTable("city");
            entity.HasKey(c => c.Id);
            entity.Property(c => c.Id).HasColumnName("ID").UseIdentityColumn();
            entity.Property(c => c.Name).HasColumnName("Name").HasMaxLength(35);
            entity.Property(c => c.CountryCode).HasColumnName("CountryCode").HasMaxLength(3);
            entity.Property(c => c.District).HasColumnName("District").HasMaxLength(20);
            entity.Property(c => c.Population).HasColumnName("Population");
            entity.HasOne(c => c.Country).WithMany(c => c.Cities).HasForeignKey(c => c.CountryCode);
        });

        // Mapeo de CountryLanguage
        modelBuilder.Entity<CountryLanguage>(entity =>
        {
            entity.ToTable("countrylanguage");
            entity.HasKey(l => new { l.CountryCode, l.Language });
            entity.Property(l => l.CountryCode).HasColumnName("CountryCode").HasMaxLength(3);
            entity.Property(l => l.Language).HasColumnName("Language").HasMaxLength(30);
            entity.Property(l => l.IsOfficial).HasColumnName("IsOfficial").HasMaxLength(1);
            entity.Property(l => l.Percentage).HasColumnName("Percentage").HasColumnType("decimal(4,1)");
            entity.HasOne(l => l.Country).WithMany(c => c.Languages).HasForeignKey(l => l.CountryCode);
        });
    }
}