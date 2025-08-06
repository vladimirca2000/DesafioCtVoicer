using System;
using System.Threading;
using System.Threading.Tasks;
using ChatBot.Api.Controllers;
using ChatBot.Application.Features.Users.Commands.CreateUser;
using ChatBot.Application.Features.Users.Commands.UpdateUserStatus;
using ChatBot.Application.Features.Users.Commands.DeleteUser;
using ChatBot.Application.Features.Users.Queries.GetUserById;
using ChatBot.Application.Common.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Api.Controllers
{
    public class UsersControllerTests
    {
        private readonly Mock<IMediator> _mediatorMock = new();
        private readonly UsersController _controller;

        public UsersControllerTests()
        {
            _controller = new UsersController(_mediatorMock.Object);
        }

        [Fact]
        public async Task CreateUser_Should_Return_Created_When_Successful()
        {
            // Arrange
            var command = new CreateUserCommand { Name = "Test User", Email = "test@email.com" };
            var response = new CreateUserResponse 
            { 
                UserId = Guid.NewGuid(), 
                Name = "Test User", 
                Email = "test@email.com",
                CreatedAt = DateTime.UtcNow
            };
            var result = Result<CreateUserResponse>.Success(response);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateUser(command, CancellationToken.None);

            // Assert
            var createdResult = actionResult.ShouldBeOfType<ObjectResult>();
            createdResult.StatusCode.ShouldBe(201);
            createdResult.Value.ShouldBe(response);
        }

        [Fact]
        public async Task CreateUser_Should_Return_BadRequest_When_Failed()
        {
            // Arrange
            var command = new CreateUserCommand { Name = "Test User", Email = "invalid-email" };
            var result = Result<CreateUserResponse>.Failure("Validation failed");

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.CreateUser(command, CancellationToken.None);

            // Assert
            var badRequestResult = actionResult.ShouldBeOfType<BadRequestObjectResult>();
            badRequestResult.StatusCode.ShouldBe(400);
        }

        [Fact]
        public async Task UpdateUserStatus_Should_Return_Ok_When_Successful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var command = new UpdateUserStatusCommand { UserId = userId, IsActive = false };
            var response = new UpdateUserStatusResponse 
            { 
                UserId = userId, 
                IsActive = false,
                UpdatedAt = DateTime.UtcNow
            };
            var result = Result<UpdateUserStatusResponse>.Success(response);

            _mediatorMock.Setup(m => m.Send(command, It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.UpdateUserStatus(userId, command, CancellationToken.None);

            // Assert
            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(response);
        }

        [Fact]
        public async Task DeleteUser_Should_Return_Ok_When_Successful()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var response = new DeleteUserResponse 
            { 
                UserId = userId, 
                IsDeleted = true,
                DeletedAt = DateTime.UtcNow
            };
            var result = Result<DeleteUserResponse>.Success(response);

            _mediatorMock.Setup(m => m.Send(It.IsAny<DeleteUserCommand>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.DeleteUser(userId, CancellationToken.None);

            // Assert
            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(response);
        }

        [Fact]
        public async Task GetUserById_Should_Return_Ok_When_User_Found()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var userDetail = new UserDetailDto 
            { 
                Id = userId, 
                Name = "Test User", 
                Email = "test@email.com",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };
            var result = Result<UserDetailDto>.Success(userDetail);

            _mediatorMock.Setup(m => m.Send(It.IsAny<GetUserByIdQuery>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(result);

            // Act
            var actionResult = await _controller.GetUserById(userId, CancellationToken.None);

            // Assert
            var okResult = actionResult.ShouldBeOfType<OkObjectResult>();
            okResult.Value.ShouldBe(userDetail);
        }
    }
}