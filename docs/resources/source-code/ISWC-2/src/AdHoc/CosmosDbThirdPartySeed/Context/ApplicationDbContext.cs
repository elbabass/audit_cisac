using CosmosDbThirdPartySeed.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;

public class ApplicationDbContext : DbContext
{
    public DbSet<Iswc> Iswc { get; set; }
    public DbSet<CachedIswcs> CachedIswcs { get; set; }
    public DbSet<UncachedIswcs> UncachedIswcs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddUserSecrets<ApplicationDbContext>()
                .Build();

        optionsBuilder.UseSqlServer(config["ConnectionString-ISWCAzureSqlDatabase"], 
                    options => options.CommandTimeout(config.GetValue<int>("ISWCAzureSqlDatabase-CommandTimeout")));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CachedIswcs>()
            .HasNoKey();

        modelBuilder.Entity<UncachedIswcs>()
            .HasNoKey();

        modelBuilder.Entity<Iswc>()
            .HasNoKey();

        modelBuilder.Entity<Iswc>()
            .ToTable("ISWC", "ISWC");
    }
}