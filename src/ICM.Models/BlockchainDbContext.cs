using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ICM.Models;

public class BlockchainDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<BlockchainInfo> BlockchainInfos { get; init; } = null!;

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<BlockchainInfo>(BuildBlockchainInfo);
    }

    private static void BuildBlockchainInfo(EntityTypeBuilder<BlockchainInfo> builder)
    {
        builder.ToTable("BlockchainInfo");
        builder.HasKey(m => m.Id);

        builder
            .Property(m => m.Id)
            .ValueGeneratedOnAdd()
            .IsRequired();

        builder
            .Property(m => m.Name)
            .HasMaxLength(10)
            .IsRequired();

        builder
            .Property(m => m.Data)
            .HasColumnType("jsonb")
            .IsRequired();

        builder
            .Property(m => m.CreatedAt)
            .HasColumnType("timestamp with time zone")
            .IsRequired();

        builder
            .HasIndex(m => new {m.Name, m.CreatedAt}, "IX_BlockchainInfo_Name_CreatedAt")
            .IsUnique()
            .IsDescending();
    }
}