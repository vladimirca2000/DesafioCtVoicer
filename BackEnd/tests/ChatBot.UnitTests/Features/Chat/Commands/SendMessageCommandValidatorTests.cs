using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Application.Features.Chat.Commands.SendMessage;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Chat.Commands
{
    public class SendMessageCommandValidatorTests
    {
        private readonly SendMessageCommandValidator _validator = new();

        [Fact]
        public void Should_Be_Valid_When_All_Fields_Are_Correct()
        {
            var command = new SendMessageCommand
            {
                ChatSessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Content = "Valid message content"
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_Have_Error_When_ChatSessionId_Is_Empty()
        {
            var command = new SendMessageCommand
            {
                ChatSessionId = Guid.Empty,
                UserId = Guid.NewGuid(),
                Content = "Valid content"
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "ChatSessionId");
        }

        [Fact]
        public void Should_Have_Error_When_UserId_Is_Empty()
        {
            var command = new SendMessageCommand
            {
                ChatSessionId = Guid.NewGuid(),
                UserId = Guid.Empty,
                Content = "Valid content"
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "UserId");
        }

        [Fact]
        public void Should_Have_Error_When_Content_Is_Empty()
        {
            var command = new SendMessageCommand
            {
                ChatSessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Content = ""
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "Content");
        }

        [Fact]
        public void Should_Have_Error_When_Content_Is_Too_Long()
        {
            var command = new SendMessageCommand
            {
                ChatSessionId = Guid.NewGuid(),
                UserId = Guid.NewGuid(),
                Content = new string('a', 5001) // Assumindo limite de 5000 caracteres
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "Content");
        }
    }
}