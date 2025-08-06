using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
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
    public class RandomResponseStrategyTests
    {
        private readonly Mock<IBotResponseRepository> _botResponseRepositoryMock = new();
        private readonly Mock<ILogger<RandomResponseStrategy>> _loggerMock = new();
        private readonly RandomResponseStrategy _strategy;

        public RandomResponseStrategyTests()
        {
            _strategy = new RandomResponseStrategy(_botResponseRepositoryMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task CanHandle_Should_Always_Return_True()
        {
            // Arrange
            var command = new ProcessUserMessageCommand { UserMessage = "qualquer mensagem" };

            // Act
            var result = await _strategy.CanHandle(command);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task GenerateResponse_Should_Return_Random_Response_When_Available()
        {
            // Arrange
            var command = new ProcessUserMessageCommand { UserMessage = "test message" };
            var botResponses = new List<BotResponse>
            {
                new BotResponse
                {
                    Id = Guid.NewGuid(),
                    Content = "Resposta aleatória 1",
                    Type = BotResponseType.Random,
                    IsActive = true,
                    IsDeleted = false
                },
                new BotResponse
                {
                    Id = Guid.NewGuid(),
                    Content = "Resposta aleatória 2",
                    Type = BotResponseType.Random,
                    IsActive = true,
                    IsDeleted = false
                }
            };

            _botResponseRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(botResponses);

            // Act
            var result = await _strategy.GenerateResponse(command);

            // Assert
            result.Value.ShouldNotBeNullOrEmpty();
            (result.Value == "Resposta aleatória 1" || result.Value == "Resposta aleatória 2").ShouldBeTrue();
        }

        [Fact]
        public async Task GenerateResponse_Should_Return_Default_When_No_Random_Responses()
        {
            // Arrange
            var command = new ProcessUserMessageCommand { UserMessage = "test message" };
            var botResponses = new List<BotResponse>();

            _botResponseRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(botResponses);

            // Act
            var result = await _strategy.GenerateResponse(command);

            // Assert
            result.Value.ShouldContain("Olá! Como posso ajudar");
        }
    }
}