using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Enums;
using ChatBot.Domain.ValueObjects;

namespace ChatBot.Infrastructure.Data.Configurations;

public class MessageConfiguration : IEntityTypeConfiguration<Message>
{
    public void Configure(EntityTypeBuilder<Message> builder)
    {
        builder.ToTable("Messages");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Content)
            .HasConversion(
                v => v.Value,
                v => MessageContent.Create(v)
            )
            .IsRequired()
            .HasMaxLength(2000);

        builder.Property(x => x.Type)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(x => x.CreatedBy)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(x => x.UpdatedBy)
            .HasMaxLength(100);

        builder.Property(x => x.DeletedBy)
            .HasMaxLength(100);

        builder.HasIndex(x => x.ChatSessionId);
        builder.HasIndex(x => x.UserId);
        builder.HasIndex(x => x.SentAt);
        builder.HasIndex(x => x.Type);
        builder.HasIndex(x => x.IsFromBot);
        builder.HasIndex(x => x.IsDeleted);

        builder.HasOne(x => x.ChatSession)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.ChatSessionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne(x => x.User)
            .WithMany(x => x.Messages)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}