using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChatBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedBotResponses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "BotResponses",
                columns: new[] { "Id", "Content", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "Keywords", "Priority", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "Olá! Como posso ajudar você hoje?", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, null, 1, 1, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111112"), "Oi! Em que posso ser útil?", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, null, 2, 1, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111113"), "Estou aqui para ajudar! O que você gostaria de saber?", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, null, 3, 1, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111114"), "Que bom te ver por aqui! Como posso te auxiliar?", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, null, 4, 1, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111115"), "Precisa de alguma informação? Estou aqui para isso!", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, null, 5, 1, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111112"));

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111113"));

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"));

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111115"));
            
        }
    }
}
