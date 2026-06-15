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
    public class ItemConfiguration:IEntityTypeConfiguration<Item>
    {
        public void Configure(EntityTypeBuilder<Item> builder)
        {
            builder.ToTable("Item");
            builder.HasKey(i => i.Id);
            builder.Property(i => i.Id)
                .UseIdentityColumn();
            builder.Property(i => i.Quantity)
                .IsRequired();
            builder.Property(i=>i.ProductId)
                .IsRequired();
        }


    }
}
