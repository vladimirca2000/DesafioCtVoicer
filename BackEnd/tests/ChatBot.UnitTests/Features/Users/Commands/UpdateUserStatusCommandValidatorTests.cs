using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Application.Features.Users.Commands.UpdateUserStatus;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Users.Commands
{
    public class UpdateUserStatusCommandValidatorTests
    {
        private readonly UpdateUserStatusCommandValidator _validator = new();

        [Fact]
        public void Should_Be_Valid_When_All_Fields_Are_Correct()
        {
            var command = new UpdateUserStatusCommand
            {
                UserId = Guid.NewGuid(),
                IsActive = true
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_Have_Error_When_UserId_Is_Empty()
        {
            var command = new UpdateUserStatusCommand
            {
                UserId = Guid.Empty,
                IsActive = true
            };

            var result = _validator.Validate(command);

            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "UserId");
        }
    }
}