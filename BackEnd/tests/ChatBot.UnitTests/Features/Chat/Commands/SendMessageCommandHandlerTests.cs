using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatBot.Application.Features.Chat.Commands.SendMessage;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.ValueObjects;
using ChatBot.Application.Common.Interfaces;
using ChatBot.Domain.Enums;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Chat.Commands
{
    public class SendMessageCommandHandlerTests
    {
        private readonly Mock<IChatSessionRepository> _chatSessionRepositoryMock = new();
        private readonly Mock<IMessageRepository> _messageRepositoryMock = new();
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly SendMessageCommandHandler _handler;

        public SendMessageCommandHandlerTests()
        {
            _handler = new SendMessageCommandHandler(
                _chatSessionRepositoryMock.Object,
                _messageRepositoryMock.Object,
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Send_Message_When_Valid_Request()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var chatSessionId = Guid.NewGuid();
            var command = new SendMessageCommand
            {
                ChatSessionId = chatSessionId,
                UserId = userId,
                Content = "Test message",
                MessageType = MessageType.Text
            };

            var user = new User { Id = userId, Name = "Test User", Email = Email.Create("test@email.com") };
            var chatSession = new ChatSession
            {
                Id = chatSessionId,
                UserId = userId,
                Status = SessionStatus.Active,
                StartedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _chatSessionRepositoryMock.Setup(r => r.GetByIdAsync(chatSessionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(chatSession);
            _messageRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value!.Content.ShouldBe(command.Content);
            result.Value.ChatSessionId.ShouldBe(chatSessionId);
            _messageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_ChatSession_Not_Found()
        {
            // Arrange
            var command = new SendMessageCommand
            {
                ChatSessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Content = "Test message"
            };

            _chatSessionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ChatSession)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Error.ShouldContain("Sessão de chat com ID");
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_Session_Not_Active()
        {
            // Arrange
            var chatSessionId = Guid.NewGuid();
            var userId = Guid.NewGuid();
            var command = new SendMessageCommand
            {
                ChatSessionId = chatSessionId,
                UserId = userId,
                Content = "Test message"
            };

            var user = new User { Id = userId, Name = "Test User", Email = Email.Create("test@email.com") };
            var chatSession = new ChatSession
            {
                Id = chatSessionId,
                UserId = userId,
                Status = SessionStatus.Ended
            };

            _chatSessionRepositoryMock.Setup(r => r.GetByIdAsync(chatSessionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(chatSession);
            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Error.ShouldContain("Não é possível enviar mensagens");
        }
    }
}