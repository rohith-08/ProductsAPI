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
    public class AppUserConfiguration: IEntityTypeConfiguration<AppUser>
    {
        public void Configure(EntityTypeBuilder<AppUser> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(u=> u.Id);

            builder.Property(u=>u.Username)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnType("NVARCHAR(100)");

            builder.Property(u=>u.Email)
                .IsRequired()
                .HasMaxLength(200)
                .HasColumnType("NVARCHAR(200)");

            builder.Property(u=>u.PasswordHash)
                .IsRequired()
                .HasColumnType("NVARCHAR(MAX)");

            builder.Property(u =>u.Role)
                .IsRequired()
                .HasMaxLength(50)
                .HasColumnType("NVARCHAR(50)")
               .HasDefaultValue("User");

            builder.Property(u => u.RefreshToken)
                .HasColumnType("NVARCHAR(MAX)")
                .IsRequired(false);

            builder.Property(u => u.RefreshTokenExpiry)
            .HasColumnType("DATETIME")
            .IsRequired(false);

            builder.Property(u => u.CreatedOn)
                .IsRequired()
                .HasColumnType("DATETIME");

            builder.HasIndex(u=>u.Email).IsUnique();
            builder.HasIndex(u=>u.Username).IsUnique();

        }
    }
}
