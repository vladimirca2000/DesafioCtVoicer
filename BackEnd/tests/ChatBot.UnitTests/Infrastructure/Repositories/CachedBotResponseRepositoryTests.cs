using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatBot.Infrastructure.Repositories;
using ChatBot.Domain.Entities;
using ChatBot.Domain.Repositories;
using ChatBot.Application.Common.Interfaces;
using Moq;
using Shouldly;
using Xunit;

namespace ChatBot.UnitTests.Infrastructure.Repositories
{
    public class CachedBotResponseRepositoryTests
    {
        private readonly Mock<IBotResponseRepository> _decoratedRepositoryMock = new();
        private readonly Mock<ICacheService> _cacheServiceMock = new();
        private readonly CachedBotResponseRepository _cachedRepository;

        public CachedBotResponseRepositoryTests()
        {
            _cachedRepository = new CachedBotResponseRepository(
                _decoratedRepositoryMock.Object,
                _cacheServiceMock.Object);
        }

        [Fact]
        public async Task GetAllAsync_Should_Return_Cached_Results_When_Cache_Exists()
        {
            // Arrange
            var cachedBotResponses = new List<BotResponse>
            {
                new BotResponse { Id = Guid.NewGuid(), Content = "Cached response", IsActive = true }
            };

            _cacheServiceMock.Setup(c => c.GetAsync<List<BotResponse>>(It.IsAny<string>()))
                .ReturnsAsync(cachedBotResponses);

            // Act
            var result = await _cachedRepository.GetAllAsync();

            // Assert
            result.ShouldBe(cachedBotResponses);
            _decoratedRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Never);
        }

        [Fact]
        public async Task GetAllAsync_Should_Call_Repository_And_Cache_When_Cache_Empty()
        {
            // Arrange
            var botResponses = new List<BotResponse>
            {
                new BotResponse { Id = Guid.NewGuid(), Content = "Repository response", IsActive = true }
            };

            _cacheServiceMock.Setup(c => c.GetAsync<List<BotResponse>>(It.IsAny<string>()))
                .ReturnsAsync((List<BotResponse>)null!);
            _decoratedRepositoryMock.Setup(r => r.GetAllAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(botResponses);
            _cacheServiceMock.Setup(c => c.SetAsync(It.IsAny<string>(), It.IsAny<List<BotResponse>>(), It.IsAny<TimeSpan>()))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _cachedRepository.GetAllAsync();

            // Assert
            result.ShouldBe(botResponses);
            _decoratedRepositoryMock.Verify(r => r.GetAllAsync(It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(c => c.SetAsync(It.IsAny<string>(), botResponses, It.IsAny<TimeSpan>()), Times.Once);
        }

        [Fact]
        public async Task AddAsync_Should_Invalidate_Cache()
        {
            // Arrange
            var botResponse = new BotResponse { Id = Guid.NewGuid(), Content = "New response" };
            _decoratedRepositoryMock.Setup(r => r.AddAsync(botResponse, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _cacheServiceMock.Setup(c => c.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cachedRepository.AddAsync(botResponse);

            // Assert
            _decoratedRepositoryMock.Verify(r => r.AddAsync(botResponse, It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_Should_Invalidate_Cache()
        {
            // Arrange
            var botResponse = new BotResponse { Id = Guid.NewGuid(), Content = "Updated response" };
            _decoratedRepositoryMock.Setup(r => r.UpdateAsync(botResponse, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _cacheServiceMock.Setup(c => c.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cachedRepository.UpdateAsync(botResponse);

            // Assert
            _decoratedRepositoryMock.Verify(r => r.UpdateAsync(botResponse, It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.AtLeast(2)); // Cache geral + específico
        }

        [Fact]
        public async Task DeleteAsync_Should_Invalidate_Cache()
        {
            // Arrange
            var id = Guid.NewGuid();
            _decoratedRepositoryMock.Setup(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            _cacheServiceMock.Setup(c => c.RemoveAsync(It.IsAny<string>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cachedRepository.DeleteAsync(id);

            // Assert
            _decoratedRepositoryMock.Verify(r => r.DeleteAsync(id, It.IsAny<CancellationToken>()), Times.Once);
            _cacheServiceMock.Verify(c => c.RemoveAsync(It.IsAny<string>()), Times.AtLeast(2)); // Cache geral + específico
        }
    }
}