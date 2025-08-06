using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatBot.Application.Features.Bot.Commands.ProcessUserMessage;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.ValueObjects;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Application.Features.Bot.Factories;
using ChatBot.Application.Features.Bot.Strategies;
using ChatBot.Domain.Enums;
using ChatBot.Application.Common.Exceptions;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Bot.Commands
{
    public class ProcessUserMessageCommandHandlerTests
    {
        private readonly Mock<IBotResponseStrategyFactory> _strategyFactoryMock = new();
        private readonly Mock<IMessageRepository> _messageRepositoryMock = new();
        private readonly Mock<IChatSessionRepository> _chatSessionRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly Mock<IBotResponseStrategy> _strategyMock = new();
        private readonly ProcessUserMessageCommandHandler _handler;

        public ProcessUserMessageCommandHandlerTests()
        {
            _handler = new ProcessUserMessageCommandHandler(
                _strategyFactoryMock.Object,
                _messageRepositoryMock.Object,
                _chatSessionRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Process_Message_And_Generate_Bot_Response()
        {
            // Arrange
            var chatSessionId = Guid.NewGuid();
            var command = new ProcessUserMessageCommand
            {
                ChatSessionId = chatSessionId,
                UserMessage = "Olá, preciso de ajuda"
            };

            var chatSession = new ChatSession
            {
                Id = chatSessionId,
                UserId = Guid.NewGuid(),
                Status = SessionStatus.Active
            };

            var botResponse = MessageContent.Create("Olá! Como posso ajudá-lo?");

            _chatSessionRepositoryMock.Setup(r => r.GetByIdAsync(chatSessionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(chatSession);
            _strategyFactoryMock.Setup(f => f.GetStrategy(command))
                .ReturnsAsync(_strategyMock.Object);
            _strategyMock.Setup(s => s.GenerateResponse(command))
                .ReturnsAsync(botResponse);
            _messageRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value!.BotMessageContent.ShouldBe(botResponse.Value);
            result.Value.ChatSessionId.ShouldBe(chatSessionId);
            _messageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_ChatSession_Not_Found()
        {
            // Arrange
            var command = new ProcessUserMessageCommand
            {
                ChatSessionId = Guid.NewGuid(),
                UserMessage = "Test message"
            };

            _chatSessionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ChatSession)null!);

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}