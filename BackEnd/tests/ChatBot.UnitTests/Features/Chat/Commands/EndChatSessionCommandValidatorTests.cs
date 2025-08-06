using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Application.Features.Chat.Commands.EndChatSession;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Chat.Commands
{
    public class EndChatSessionCommandValidatorTests
    {
        private readonly EndChatSessionCommandValidator _validator = new();

        [Fact]
        public void Should_Be_Valid_When_ChatSessionId_Is_Valid()
        {
            var command = new EndChatSessionCommand
            {
                ChatSessionId = Guid.NewGuid()
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_Have_Error_When_ChatSessionId_Is_Empty()
        {
            var command = new EndChatSessionCommand
            {
                ChatSessionId = Guid.Empty
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "ChatSessionId");
        }
    }
}