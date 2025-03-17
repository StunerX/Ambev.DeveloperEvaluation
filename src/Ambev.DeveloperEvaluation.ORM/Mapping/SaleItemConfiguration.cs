using Ambev.DeveloperEvaluation.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Ambev.DeveloperEvaluation.ORM.Mapping;

public class SaleItemConfiguration : IEntityTypeConfiguration<SaleItem>
{
    public void Configure(EntityTypeBuilder<SaleItem> builder)
    {
        builder.ToTable("SaleItems");
        
        builder.HasQueryFilter(item => item.DeletedAt == null);

        builder.HasKey(si => si.Id);
        
        builder.Property(si => si.ProductId)
            .IsRequired();

        builder.Property(si => si.ProductName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(si => si.Quantity)
            .IsRequired();

        builder.Property(si => si.UnitPrice)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(si => si.Discount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Property(si => si.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        
        builder.Property(x => x.IsCancelled).IsRequired();
        builder.Property(x => x.DeletedAt);

        builder.Property(x => x.SaleId).IsRequired();
    }
}