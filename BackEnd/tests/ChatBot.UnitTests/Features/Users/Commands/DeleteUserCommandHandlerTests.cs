using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Application.Features.Users.Commands.DeleteUser;
using ChatBot.Application.Common.Models;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Domain.ValueObjects;
using ChatBot.Application.Common.Interfaces;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Features.Users.Commands
{
    public class DeleteUserCommandHandlerTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock = new();
        private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
        private readonly DeleteUserCommandHandler _handler;

        public DeleteUserCommandHandlerTests()
        {
            _handler = new DeleteUserCommandHandler(
                _userRepositoryMock.Object,
                _unitOfWorkMock.Object);
        }

        [Fact]
        public async Task Handle_Should_Delete_User_When_User_Exists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new DeleteUserCommand { UserId = userId };
            var user = new User 
            { 
                Id = userId, 
                Name = "Test User", 
                Email = Email.Create("test@email.com"),
                IsActive = true
            };

            _userRepositoryMock.Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            _userRepositoryMock.Setup(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _unitOfWorkMock.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(1));

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            result.IsSuccess.ShouldBeTrue();
            result.Value.ShouldNotBeNull();
            result.Value!.UserId.ShouldBe(userId);
            result.Value.IsDeleted.ShouldBeTrue();
            _userRepositoryMock.Verify(r => r.DeleteAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_Should_Throw_NotFoundException_When_User_Not_Found()
        {
            // Arrange
            var command = new DeleteUserCommand { UserId = Guid.NewGuid() };
            _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User)null!);

            // Act & Assert
            await Should.ThrowAsync<ChatBot.Application.Common.Exceptions.NotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        }
    }
}