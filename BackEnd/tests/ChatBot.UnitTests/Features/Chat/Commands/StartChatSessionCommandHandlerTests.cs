using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Application.Features.Chat.Commands.StartChatSession;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.ValueObjects;
using ChatBot.Domain.Enums;
using ChatBot.Application.Common.Interfaces;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Chat.Commands
{
    public class StartChatSessionCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IChatSessionRepository> _chatSessionRepositoryMock = new();
        private readonly Mock<IMessageRepository> _messageRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly StartChatSessionCommandHandler _handler;

        public StartChatSessionCommandHandlerTests()
        {
            _handler = new StartChatSessionCommandHandler(
                _userRepositoryMock.Object,
                _chatSessionRepositoryMock.Object,
                _messageRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Start_Chat_Session_When_User_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new StartChatSessionCommand 
            { 
                UserId = userId,
                InitialMessageContent = "Hello, I need help"
            };
            var user = new User 
            { 
                Id = userId, 
                Name = "Test User", 
                Email = Email.Create("test@email.com"),
                IsActive = true
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _chatSessionRepositoryMock.Setup(r => r.AddAsync(It.IsAny<ChatSession>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _messageRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value!.UserId.ShouldBe(userId);
            result.Value.InitialMessage.ShouldBe(command.InitialMessageContent);
            _chatSessionRepositoryMock.Verify(r => r.AddAsync(It.IsAny<ChatSession>(), It.IsAny<CancellationToken>()), Times.Once);
            _messageRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Message>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.AtLeastOnce);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_User_Not_Found()
        {
            // Arrange
            var command = new StartChatSessionCommand 
            { 
                UserId = Guid.NewGuid(),
                InitialMessageContent = "Test message"
            };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Error.ShouldContain("Usuário com ID");
        }
    }
}