using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatBot.Application.Features.Bot.Factories;
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Application.Features.Bot.Strategies;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Bot.Factories
{
    public class BotResponseStrategyFactoryTests
    {
        private readonly Mock<ILogger<BotResponseStrategyFactory>> _loggerMock = new();

        [Fact]
        public async Task GetStrategy_Should_Throw_When_No_Strategies_Available()
        {
            // Arrange
            var emptyFactory = new BotResponseStrategyFactory(new List<IBotResponseStrategy>(), _loggerMock.Object);
            var command = new ProcessUserMessageCommand { UserMessage = "test" };

            // Act & Assert
            await Should.ThrowAsync<InvalidOperationException>(() => emptyFactory.GetStrategy(command));
        }
    }
}