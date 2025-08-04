using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ChatBot.Domain.Entities;
using ChatBot.Domain.ValueObjects; // Necessário para Email

namespace ChatBot.Infrastructure.Data.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(x => x.Id);

        // Properties
        builder.Property(x => x.Name)
            .IsRequired()
            .HasMaxLength(100);

        // Mapeamento do Value Object Email para string no banco de dados
        builder.Property(x => x.Email)
            .HasConversion(
                v => v.Value, // Como salvar no banco (Email VO -> string)
                v => Email.Create(v) // Como carregar do banco (string -> Email VO)
            )
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(x => x.IsActive)
            .IsRequired()
            .HasDefaultValue(true);

        // Audit Properties
        builder.Property(x => x.CreatedAt)
            .IsRequired();

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(100);

        // Soft Delete Properties
        builder.Property(x => x.IsDeleted)
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(x => x.DeletedBy)
            .HasMaxLength(100);

        // Indexes - Usando aspas duplas corretas para PostgreSQL
        builder.HasIndex(x => x.Email) // O índice será aplicado à coluna que armazena o valor (string)
            .IsUnique()
            .HasDatabaseName("IX_Users_Email_Unique")
            .HasFilter(@"""IsDeleted"" = false"); // Case-sensitive correto

        builder.HasIndex(x => x.IsDeleted)
            .HasDatabaseName("IX_Users_IsDeleted");

        builder.HasIndex(x => x.IsActive)
            .HasDatabaseName("IX_Users_IsActive");

        // Relationships
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