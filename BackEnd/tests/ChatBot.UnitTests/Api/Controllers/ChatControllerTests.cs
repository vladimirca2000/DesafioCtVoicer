using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Api.Controllers;
using ChatBot.Application.Features.Chat.Commands.StartChatSession;
using ChatBot.Application.Features.Chat.Commands.SendMessage;
using ChatBot.Application.Features.Chat.Commands.EndChatSession;
using ChatBot.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Api.Controllers
{
    public class ChatControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock = new();
        private readonly ChatController _controller;

        public ChatControllerTests()
        {
            _controller = new ChatController(_mediatorMock.Object);
        }

        [Fact]
        public async Task StartSession_Should_Return_Ok_When_Successful()
        {
            // Arrange
            var command = new StartChatSessionCommand 
            { 
                UserId = Guid.NewGuid(),
                InitialMessageContent = "Hello"
            };
            var response = new StartChatSessionResponse 
            { 
                ChatSessionId = Guid.NewGuid(), 
                UserId = command.UserId.Value,
                StartedAt = DateTime.UtcNow,
                InitialMessage = "Hello"
            };
            var result = Result<StartChatSessionResponse>.Success(response);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.StartSession(command, CancellationToken.None);

            // Assert
            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(response);
        }

        [Fact]
        public async Task SendMessage_Should_Return_Ok_When_Successful()
        {
            // Arrange
            var command = new SendMessageCommand 
            { 
                ChatSessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Content = "Test message"
            };
            var response = new SendMessageResponse 
            { 
                MessageId = Guid.NewGuid(),
                ChatSessionId = command.ChatSessionId,
                UserId = command.UserId,
                Content = command.Content,
                SentAt = DateTime.UtcNow
            };
            var result = Result<SendMessageResponse>.Success(response);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.SendMessage(command, CancellationToken.None);

            // Assert
            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(response);
        }

        [Fact]
        public async Task EndSession_Should_Return_Ok_When_Successful()
        {
            // Arrange
            var chatSessionId = Guid.NewGuid();
            var command = new EndChatSessionCommand 
            { 
                ChatSessionId = chatSessionId
            };
            var response = new EndChatSessionResponse 
            { 
                ChatSessionId = chatSessionId,
                EndedAt = DateTime.UtcNow
            };
            var result = Result<EndChatSessionResponse>.Success(response);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.EndSession(command, CancellationToken.None);

            // Assert
            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(response);
        }
    }
}