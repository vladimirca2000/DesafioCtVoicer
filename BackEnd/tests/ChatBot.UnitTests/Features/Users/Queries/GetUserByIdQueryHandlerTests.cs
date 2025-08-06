using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Application.Features.Users.Queries.GetUserById;
using ChatBot.Application.Common.Exceptions;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.ValueObjects;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Users.Queries
{
    public class GetUserByIdQueryHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly GetUserByIdQueryHandler _handler;

        public GetUserByIdQueryHandlerTests()
        {
            _handler = new GetUserByIdQueryHandler(_userRepositoryMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Return_User_When_User_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserByIdQuery { UserId = userId };
            var user = new User 
            { 
                Id = userId, 
                Name = "Test User", 
                Email = Email.Create("test@email.com"),
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            // Act
            var result = await _handler.Handle(query, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value!.Id.ShouldBe(userId);
            result.Value.Name.ShouldBe(user.Name);
            result.Value.Email.ShouldBe(user.Email.Value);
            result.Value.IsActive.ShouldBe(user.IsActive);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_User_Not_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var query = new GetUserByIdQuery { UserId = userId };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null!);

            // Act & Assert
            await Should.ThrowAsync<NotFoundException>(() => _handler.Handle(query, CancellationToken.None));
        }
    }
}