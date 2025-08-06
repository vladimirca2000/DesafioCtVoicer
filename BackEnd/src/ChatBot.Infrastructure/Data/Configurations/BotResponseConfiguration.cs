using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Enums;

namespace ChatBot.Infrastructure.Data.Configurations;

public class BotResponseConfiguration : IEntityTypeConfiguration<BotResponse>
{
    public void Configure(EntityTypeBuilder<BotResponse> builder)
    {
        builder.ToTable("BotResponses");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .IsRequired()
            .HasMaxLength(1000);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.Keywords)
            .HasMaxLength(500);

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.DeletedBy)
            .HasMaxLength(100);

        
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.IsActive);
        builder.HasIndex(x => x.Priority);
        builder.HasIndex(x => x.IsDeleted);

        // Seed data
        builder.HasData(
            // Saudações aleatórias
            new BotResponse
            {
                Id = new Guid("11111111-1111-1111-1111-111111111111"),
                Content = "Olá! Como posso ajudar você hoje?",
                Type = BotResponseType.Random,
                IsActive = true,
                Priority = 1,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                IsDeleted = false
            },
            new BotResponse
            {
                Id = new Guid("11111111-1111-1111-1111-111111111112"),
                Content = "Oi! Em que posso ser útil?",
                Type = BotResponseType.Random,
                IsActive = true,
                Priority = 2,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                IsDeleted = false
            },
            new BotResponse
            {
                Id = new Guid("11111111-1111-1111-1111-111111111113"),
                Content = "Estou aqui para ajudar! O que você gostaria de saber?",
                Type = BotResponseType.Random,
                IsActive = true,
                Priority = 3,
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                CreatedBy = "System",
                IsDeleted = false
            }
        );
    }
}