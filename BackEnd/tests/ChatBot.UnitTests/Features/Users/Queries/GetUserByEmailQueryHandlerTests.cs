using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatBot.Application.Features.Users.Queries.GetUserByEmail;
using ChatBot.Application.Common.Exceptions;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.ValueObjects;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Users.Queries
{
    public class GetUserByEmailQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly GetUserByEmailQueryHandler _handler;

        public GetUserByEmailQueryHandlerTests()
        {
            _handler = new GetUserByEmailQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_User_When_User_Exists()
        {
            // Arrange
            var email = Email.Create("test@email.com");
            var query = new GetUserByEmailQuery { Email = "test@email.com" };
            var user = new User 
            { 
                Id = Guid.NewGuid(), 
                Name = "Test User", 
                Email = email,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value!.Id.ShouldBe(user.Id);
            result.Value.Name.ShouldBe(user.Name);
            result.Value.Email.ShouldBe(user.Email.Value);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_User_Not_Exists()
        {
            // Arrange
            var query = new GetUserByEmailQuery { Email = "notfound@email.com" };

            _userRepositoryMock.Setup(r => r.GetUserByEmailAsync(It.IsAny<Email>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null!);

            // Act & Assert
            await Should.ThrowAsync<ChatBot.Application.Common.Exceptions.NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}