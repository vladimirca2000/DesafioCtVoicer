using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ChatBot.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ImproveContextualResponses : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111114"));

            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111115"));

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Content",
                value: "Claro! Estou aqui para ajudar. Você pode me perguntar sobre nossos produtos, serviços, preços, horários de funcionamento, ou formas de contato. O que você gostaria de saber?");

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222223"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Nossa equipe de suporte está sempre disponível! Para questões técnicas, você pode descrever seu problema aqui ou entrar em contato diretamente pelo telefone (11) 1234-5678. Como posso te direcionar melhor?", "suporte,apoio,atendimento,assistencia,assistência,problema,erro,bug" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222224"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Posso te informar sobre nossos produtos, serviços, preços, horários de atendimento e formas de contato. Sobre qual tema específico você gostaria de saber mais?", "informação,informações,info,saber,conhecer,duvida,dúvida,detalhes" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222225"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Temos uma ampla gama de produtos e serviços disponíveis! Para te ajudar melhor, você poderia me dizer que tipo de solução você está procurando? Posso te dar detalhes sobre características, preços e disponibilidade.", "produto,produtos,serviço,serviços,oferta,ofertas,venda,vendas,comprar,adquirir" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222226"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Para informações detalhadas sobre preços e condições especiais, posso te conectar com nossa equipe comercial que fará um orçamento personalizado. Qual produto ou serviço te interessa? Também posso adiantar algumas informações gerais de valores.", "preço,preços,valor,valores,custo,custos,quanto custa,barato,caro,orçamento,investimento" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222227"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Nosso horário de atendimento é:\n📅 Segunda a Sexta: 8h às 18h\n📅 Sábados: 8h às 12h\n📅 Domingos: Fechado\n\nEste chat está disponível 24h! Posso ajudar com mais alguma informação?", "horário,horarios,funcionamento,aberto,fechado,atendimento,quando,abre,fecha" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222228"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Você pode entrar em contato conosco através de:\n📞 Telefone: (11) 1234-5678\n📧 Email: contato@empresa.com\n💬 WhatsApp: (11) 99999-9999\n🌐 Site: www.empresa.com\n\nTambém posso te ajudar diretamente por este chat! O que você precisa?", "contato,telefone,email,falar,ligar,whatsapp,comunicar,conversar" });

            migrationBuilder.InsertData(
                table: "BotResponses",
                columns: new[] { "Id", "Content", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "Keywords", "Priority", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[] { new Guid("22222222-2222-2222-2222-222222222229"), "Posso te orientar sobre nossos processos! Me diga qual procedimento você gostaria de entender melhor: como fazer um pedido, processo de compra, prazos de entrega, políticas de troca, ou qualquer outro processo específico.", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, "como,fazer,processo,procedimento,passo,etapa,orientação,tutorial", 1, 2, null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222229"));

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "Content",
                value: "Claro! Estou aqui para ajudar. Você pode me perguntar sobre nossos serviços, horários de funcionamento, ou qualquer dúvida que tiver.");

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
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Você gostaria de saber sobre nossos produtos, serviços, horários ou tem alguma dúvida específica?", "informação,informações,info,saber,conhecer,duvida,dúvida" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222225"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Temos uma variedade de produtos e serviços disponíveis! Gostaria de saber sobre algum em específico?", "produto,produtos,serviço,serviços,oferta,ofertas,venda,vendas" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222226"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Para informações sobre preços e valores, posso te direcionar para nossa equipe comercial. Qual produto ou serviço te interessa?", "preço,preços,valor,valores,custo,custos,quanto custa,barato,caro" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222227"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Nosso horário de atendimento é de segunda a sexta das 8h às 18h, e sábados das 8h às 12h. Posso ajudar com mais alguma coisa?", "horário,horarios,funcionamento,aberto,fechado,atendimento" });

            migrationBuilder.UpdateData(
                table: "BotResponses",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222228"),
                columns: new[] { "Content", "Keywords" },
                values: new object[] { "Para entrar em contato conosco, você pode usar este chat, ligar para (11) 1234-5678 ou enviar um email para contato@empresa.com.", "contato,telefone,email,falar,ligar,whatsapp" });

            migrationBuilder.InsertData(
                table: "BotResponses",
                columns: new[] { "Id", "Content", "CreatedAt", "CreatedBy", "DeletedAt", "DeletedBy", "IsActive", "IsDeleted", "Keywords", "Priority", "Type", "UpdatedAt", "UpdatedBy" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111114"), "Que bom te ver por aqui! Como posso te auxiliar?", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, null, 4, 1, null, null },
                    { new Guid("11111111-1111-1111-1111-111111111115"), "Precisa de alguma informação? Estou aqui para isso!", new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "System", null, null, true, false, null, 5, 1, null, null }
                });
        }
    }
}
