using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using ChatBot.Application.Features.Bot.Strategies;
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Bot.Strategies
{
    public class KeywordBasedResponseStrategyTests
    {
        private readonly Mock<IBotResponseRepository> _botResponseRepositoryMock = new();
        private readonly Mock<ILogger<KeywordBasedResponseStrategy>> _loggerMock = new();
        private readonly KeywordBasedResponseStrategy _strategy;

        public KeywordBasedResponseStrategyTests()
        {
            _strategy = new KeywordBasedResponseStrategy(
                _botResponseRepositoryMock.Object,
                _loggerMock.Object);
        }

        [Fact]
        public async Task CanHandle_Should_Return_True_When_Keywords_Match()
        {
            // Arrange
            var command = new ProcessUserMessageCommand { UserMessage = "qual o horário de funcionamento?" };
            var botResponses = new List<BotResponse>
            {
                new BotResponse
                {
                    Id = Guid.NewGuid(),
                    Content = "Funcionamos de segunda a sexta das 8h às 18h",
                    Keywords = "horário,funcionamento,aberto,fechado",
                    Type = BotResponseType.KeywordBased,
                    IsActive = true,
                    IsDeleted = false
                }
            };

            _botResponseRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(botResponses);

            // Act
            var result = await _strategy.CanHandle(command);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task CanHandle_Should_Return_False_When_No_Keywords_Match()
        {
            // Arrange
            var command = new ProcessUserMessageCommand { UserMessage = "mensagem sem palavras-chave específicas" };
            var botResponses = new List<BotResponse>
            {
                new BotResponse
                {
                    Id = Guid.NewGuid(),
                    Content = "Resposta sobre preços",
                    Keywords = "preço,valor,custo",
                    Type = BotResponseType.KeywordBased,
                    IsActive = true,
                    IsDeleted = false
                }
            };

            _botResponseRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(botResponses);

            // Act
            var result = await _strategy.CanHandle(command);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task GenerateResponse_Should_Return_Best_Match_Response()
        {
            // Arrange
            var command = new ProcessUserMessageCommand { UserMessage = "qual o preço dos produtos?" };
            var botResponses = new List<BotResponse>
            {
                new BotResponse
                {
                    Id = Guid.NewGuid(),
                    Content = "Nossos preços variam conforme o produto. Entre em contato para mais informações.",
                    Keywords = "preço,valor,custo,produtos",
                    Type = BotResponseType.KeywordBased,
                    IsActive = true,
                    IsDeleted = false,
                    Priority = 1
                },
                new BotResponse
                {
                    Id = Guid.NewGuid(),
                    Content = "Resposta genérica",
                    Keywords = "ajuda,geral",
                    Type = BotResponseType.KeywordBased,
                    IsActive = true,
                    IsDeleted = false,
                    Priority = 2
                }
            };

            _botResponseRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(botResponses);

            // Act
            var result = await _strategy.GenerateResponse(command);

            // Assert
            result.Value.ShouldContain("preços variam conforme");
        }

        [Fact]
        public async Task GenerateResponse_Should_Return_Default_When_No_Match()
        {
            // Arrange
            var command = new ProcessUserMessageCommand { UserMessage = "mensagem completamente irrelevante xyz" };
            var botResponses = new List<BotResponse>();

            _botResponseRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(botResponses);

            // Act
            var result = await _strategy.GenerateResponse(command);

            // Assert
            result.Value.ShouldContain("Não entendi sua pergunta");
        }
    }
}