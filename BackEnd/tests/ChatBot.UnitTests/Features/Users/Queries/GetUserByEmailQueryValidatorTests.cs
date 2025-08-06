using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Application.Features.Users.Queries.GetUserByEmail;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Users.Queries
{
    public class GetUserByEmailQueryValidatorTests
    {
        private readonly GetUserByEmailQueryValidator _validator = new();

        [Fact]
        public void Should_Be_Valid_When_Email_Is_Valid()
        {
            var query = new GetUserByEmailQuery { Email = "test@email.com" };
            var result = _validator.Validate(query);
            result.IsValid.ShouldBeTrue();
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Empty()
        {
            var query = new GetUserByEmailQuery { Email = "" };
            var result = _validator.Validate(query);
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "Email");
        }

        [Fact]
        public void Should_Have_Error_When_Email_Is_Invalid_Format()
        {
            var query = new GetUserByEmailQuery { Email = "invalid-email" };
            var result = _validator.Validate(query);
            result.IsValid.ShouldBeFalse();
            result.Errors.ShouldContain(e => e.PropertyName == "Email");
        }
    }
}