using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProductsAPI.Domain.Entities;

namespace ProductsAPI.Infrastructure.Data.Configurations
{
    public class ProductConfiguration : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Product");
            builder.HasKey(p => p.Id);
            builder.Property(p => p.Id).UseIdentityColumn();
            builder.Property(p => p.ProductName).IsRequired().HasMaxLength(255).HasColumnType("NVARCHAR(255)");
            builder.Property(p => p.CreatedBy).IsRequired().HasMaxLength(100).HasColumnType("NVARCHAR(100)");
            builder.Property(p=>p.CreatedOn).IsRequired().HasColumnType("DATETIME");
            builder.Property(p=>p.ModifiedOn)
                           .HasColumnType("DATETIME")
                           .IsRequired(false);

            builder.HasMany(p=>p.Items)
                  .WithOne(i => i.Product)
                  .HasForeignKey(i => i.ProductId)
                  .OnDelete(DeleteBehavior.Cascade);
        }
    }
    
}
