using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ChatBot.Domain.Entities;
using ChatBot.Domain.ValueObjects;

namespace ChatBot.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.Email)
            .HasConversion(
                v => v.Value,
                v => Email.Create(v)
            )
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedBy)
            .HasMaxLength(100);

        builder.HasIndex(x => x.Email)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email_Unique")
            .HasFilter(@"""IsDeleted"" = false");

        builder.HasIndex(x => x.IsDeleted)
            .HasDatabaseName("IX_Users_IsDeleted");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        builder.HasMany(x => x.ChatSessions)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(x => x.Messages)
            .WithOne(x => x.User)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}