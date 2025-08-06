using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Application.Features.Chat.Commands.EndChatSession;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.Enums;
using ChatBot.Application.Common.Interfaces;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Chat.Commands
{
    public class EndChatSessionCommandHandlerTests
    {
        private readonly Mock<IChatSessionRepository> _chatSessionRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly EndChatSessionCommandHandler _handler;

        public EndChatSessionCommandHandlerTests()
        {
            _handler = new EndChatSessionCommandHandler(
                _chatSessionRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_End_Chat_Session_When_Session_Exists_And_Active()
        {
            // Arrange
            var chatSessionId = Guid.NewGuid();
            var command = new EndChatSessionCommand 
            { 
                ChatSessionId = chatSessionId
            };
            var chatSession = new ChatSession 
            { 
                Id = chatSessionId, 
                UserId = Guid.NewGuid(),
                Status = SessionStatus.Active,
                StartedAt = DateTime.UtcNow
            };

            _chatSessionRepositoryMock.Setup(r => r.GetByIdAsync(chatSessionId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(chatSession);
            _chatSessionRepositoryMock.Setup(r => r.UpdateAsync(It.IsAny<ChatSession>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value!.ChatSessionId.ShouldBe(chatSessionId);
            chatSession.Status.ShouldBe(SessionStatus.Ended);
            _chatSessionRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<ChatSession>(), It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Return_Failure_When_Session_Not_Found()
        {
            // Arrange
            var command = new EndChatSessionCommand { ChatSessionId = Guid.NewGuid() };
            _chatSessionRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((ChatSession)null!);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeFalse();
            result.Error.ShouldNotBeNullOrEmpty();
        }
    }
}