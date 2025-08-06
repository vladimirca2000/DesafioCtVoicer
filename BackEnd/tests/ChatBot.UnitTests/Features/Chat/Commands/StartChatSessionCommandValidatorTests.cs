using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Application.Features.Chat.Commands.StartChatSession;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Chat.Commands
{
    public class StartChatSessionCommandValidatorTests
    {
        private readonly StartChatSessionCommandValidator _validator = new();

        [Fact]
        public void Should_Be_Valid_When_UserId_Is_Valid()
        {
            var command = new StartChatSessionCommand
            {
                UserId = Guid.NewGuid(),
                InitialMessageContent = "Hello"
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_Have_Error_When_InitialMessageContent_Is_Empty()
        {
            var command = new StartChatSessionCommand
            {
                UserId = Guid.NewGuid(),
                InitialMessageContent = ""
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "InitialMessageContent");
        }

        [Fact]
        public void Should_Have_Error_When_UserName_Is_Empty_And_UserId_Is_Null()
        {
            var command = new StartChatSessionCommand
            {
                UserId = null,
                UserName = "",
                InitialMessageContent = "Hello"
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "UserName");
        }
    }
}