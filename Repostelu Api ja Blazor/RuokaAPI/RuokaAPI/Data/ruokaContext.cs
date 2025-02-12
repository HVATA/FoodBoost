using Microsoft.EntityFrameworkCore;

using RuokaAPI.Properties.Model;

namespace RuokaAPI.Data; 

public class ruokaContext : DbContext
{
    public ruokaContext(DbContextOptions<ruokaContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Resepti>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Tekijäid)
                .IsRequired();
            entity.Property(r => r.Valmistuskuvaus)
                .HasMaxLength(1024);
            entity.Property(r => r.Nimi)
                .HasMaxLength(50);
            entity.Property(r => r.Kuva1);
            entity.Property(r => r.Kuva2);
            entity.Property(r => r.Kuva3);
            entity.Property(r => r.Kuva4);
            entity.Property(r => r.Kuva5);
            entity.Property(r => r.Kuva6);
            entity.Property(r => r.Katseluoikeus);

            entity.HasMany(r => r.AinesosanMaara)
                  .WithOne(a => a.Resepti);

            entity.HasMany(r => r.Avainsanat)
                  .WithMany(a => a.Reseptit);

            entity.HasMany(r => r.Arvostelut)
                 .WithOne(a => a.Resepti);
        });

        modelBuilder.Entity<Avainsana>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Sana)
                .IsRequired()
                .HasMaxLength(30);
        });

        modelBuilder.Entity<Ainesosa>(entity =>
        {
            entity.HasKey(r => r.Id);
            entity.Property(r => r.Nimi)
                .IsRequired()
                .HasMaxLength(30);
        });

        // Add configuration for ReseptiAinesosa
        modelBuilder.Entity<ReseptiAinesosa>(entity =>
        {
            entity.HasKey(ra => new { ra.ReseptiId, ra.AinesosaId });
            entity.ToTable("AinesosaResepti");
            entity.HasOne(ra => ra.Resepti)
                  .WithMany(r => r.AinesosanMaara)
                  .HasForeignKey(ra => ra.ReseptiId);

            entity.HasOne(ra => ra.Ainesosa)
                  .WithMany(a => a.AinesosanMaara)
                  .HasForeignKey(ra => ra.AinesosaId);

            entity.Property(ra => ra.Maara)
                  .HasMaxLength(50)
                  .IsRequired();
        });
    }

    public DbSet<Avainsana> Avainsanat { get; set; }
    public DbSet<Kayttaja> Kayttajat { get; set; }
    public DbSet<Resepti> Reseptit { get; set; }
    public DbSet<Ainesosa> Ainesosat { get; set; }
    public DbSet<Suosikit> Suosikit { get; set; }
    public DbSet<Arvostelu> Arvostelut { get; set; }
    public DbSet<ReseptiAinesosa> ReseptiAinesosat { get; set; } // Add DbSet for ReseptiAinesosa
}
