using System;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using ChatBot.Infrastructure.Services;
using ChatBot.Application.Common.Interfaces;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;
using System.Text;

namespace ChatBot.UnitTests.Infrastructure.Services
{
    public class CacheServiceTests
    {
        private readonly Mock<IDistributedCache> _distributedCacheMock = new();
        private readonly CacheService _cacheService;

        public CacheServiceTests()
        {
            _cacheService = new CacheService(_distributedCacheMock.Object);
        }

        [Fact]
        public async Task GetAsync_Should_Return_Cached_Value_When_Key_Exists()
        {
            // Arrange
            var key = "test-key";
            var cachedValue = "cached-value";
            var cachedJson = $"\"{cachedValue}\""; // JSON serialized string
            var cachedBytes = Encoding.UTF8.GetBytes(cachedJson);

            _distributedCacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync(cachedBytes);

            // Act
            var result = await _cacheService.GetAsync<string>(key);

            // Assert
            result.ShouldBe(cachedValue);
        }

        [Fact]
        public async Task GetAsync_Should_Return_Default_When_Key_Not_Exists()
        {
            // Arrange
            var key = "non-existent-key";
            _distributedCacheMock.Setup(c => c.GetAsync(key, It.IsAny<CancellationToken>()))
                .ReturnsAsync((byte[])null!);

            // Act
            var result = await _cacheService.GetAsync<string>(key);

            // Assert
            result.ShouldBeNull();
        }

        [Fact]
        public async Task SetAsync_Should_Store_Value_In_Cache()
        {
            // Arrange
            var key = "test-key";
            var value = "test-value";
            var timeSpan = TimeSpan.FromMinutes(30);

            _distributedCacheMock.Setup(c => c.SetAsync(
                key, 
                It.IsAny<byte[]>(), 
                It.IsAny<DistributedCacheEntryOptions>(), 
                It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.SetAsync(key, value, timeSpan);

            // Assert
            _distributedCacheMock.Verify(c => c.SetAsync(
                key, 
                It.IsAny<byte[]>(), 
                It.IsAny<DistributedCacheEntryOptions>(), 
                It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task RemoveAsync_Should_Remove_Key_From_Cache()
        {
            // Arrange
            var key = "test-key";
            _distributedCacheMock.Setup(c => c.RemoveAsync(key, It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            // Act
            await _cacheService.RemoveAsync(key);

            // Assert
            _distributedCacheMock.Verify(c => c.RemoveAsync(key, It.IsAny<CancellationToken>()), Times.Once);
        }
    }
}