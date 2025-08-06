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
                    { new Guid("11111111-1111-1111-1111-111111111115"), "Precisa de alguma informação? Estou aqui para isso!", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, null, 5, 1, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222221"), "Claro! Estou aqui para ajudar. Você pode me perguntar sobre nossos serviços, horários de funcionamento, ou qualquer dúvida que tiver.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, "ajuda", 1, 2, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "Nossa equipe de suporte está sempre disponível! Posso te ajudar com informações gerais ou te direcionar para o setor adequado.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, "suporte", 1, 2, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222223"), "Olá! Seja muito bem-vindo(a)! Como posso te ajudar hoje?", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, "oi,olá,hello,bom dia,boa tarde,boa noite", 1, 2, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222224"), "Você gostaria de saber sobre nossos produtos, serviços, horários ou tem alguma dúvida específica?", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, "informação,informações,info", 1, 2, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222225"), "Temos uma variedade de produtos e serviços disponíveis! Gostaria de saber sobre algum em específico?", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, "produto,produtos,serviço,serviços", 1, 2, null, null }
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

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"));

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"));

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222224"));

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222225"));
        }
    }
}
