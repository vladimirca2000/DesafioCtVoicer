using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChatBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBotResponsesWithMoreKeywords : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Olá! Seja muito bem-vindo(a)! Como posso te ajudar hoje?", "oi,olá,hello,hi,bom dia,boa tarde,boa noite,ola" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Claro! Estou aqui para ajudar. Você pode me perguntar sobre nossos serviços, horários de funcionamento, ou qualquer dúvida que tiver.", "ajuda,help,socorro,auxilio,auxiliar,preciso" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Nossa equipe de suporte está sempre disponível! Posso te ajudar com informações gerais ou te direcionar para o setor adequado.", "suporte,apoio,atendimento,assistencia,assistência" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222224"),
                column: "Keywords",
                value: "informação,informações,info,saber,conhecer,duvida,dúvida");

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222225"),
                column: "Keywords",
                value: "produto,produtos,serviço,serviços,oferta,ofertas,venda,vendas");

            migrationBuilder.InsertData(
                table: "BotResponses",
                columns: new[] { "Id", "Content", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "Keywords", "Priority", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("22222222-2222-2222-2222-222222222226"), "Para informações sobre preços e valores, posso te direcionar para nossa equipe comercial. Qual produto ou serviço te interessa?", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, "preço,preços,valor,valores,custo,custos,quanto custa,barato,caro", 1, 2, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222227"), "Nosso horário de atendimento é de segunda a sexta das 8h às 18h, e sábados das 8h às 12h. Posso ajudar com mais alguma coisa?", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, "horário,horarios,funcionamento,aberto,fechado,atendimento", 1, 2, null, null },
                    { new Guid("22222222-2222-2222-2222-222222222228"), "Para entrar em contato conosco, você pode usar este chat, ligar para (11) 1234-5678 ou enviar um email para contato@empresa.com.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, "contato,telefone,email,falar,ligar,whatsapp", 1, 2, null, null }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222226"));

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222227"));

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222228"));

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222221"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Claro! Estou aqui para ajudar. Você pode me perguntar sobre nossos serviços, horários de funcionamento, ou qualquer dúvida que tiver.", "ajuda" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Nossa equipe de suporte está sempre disponível! Posso te ajudar com informações gerais ou te direcionar para o setor adequado.", "suporte" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Olá! Seja muito bem-vindo(a)! Como posso te ajudar hoje?", "oi,olá,hello,bom dia,boa tarde,boa noite" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222224"),
                column: "Keywords",
                value: "informação,informações,info");

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222225"),
                column: "Keywords",
                value: "produto,produtos,serviço,serviços");
        }
    }
}
