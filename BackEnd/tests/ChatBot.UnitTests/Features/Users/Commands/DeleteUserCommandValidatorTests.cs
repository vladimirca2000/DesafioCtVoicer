using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Application.Features.Users.Commands.DeleteUser;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Users.Commands
{
    public class DeleteUserCommandValidatorTests
    {
        private readonly DeleteUserCommandValidator _validator = new();

        [Fact]
        public void Should_Be_Valid_When_UserId_Is_Valid()
        {
            var command = new DeleteUserCommand
            {
                UserId = Guid.NewGuid()
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_Have_Error_When_UserId_Is_Empty()
        {
            var command = new DeleteUserCommand
            {
                UserId = Guid.Empty
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "UserId");
        }
    }
}