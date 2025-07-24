namespace UrlShortener.UnitTests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Services;
using Xunit;
using Moq;

public class ShortenedLinkService_GenerateShortLinkAsync_UnitTests
{
    [Fact]
    public async Task FirstAttemptSuccess()
    {
        var mockStorage = new Mock<IShortenedLinkStorage>();
        mockStorage.Setup(s => s.AddAsync(It.IsAny<string>(),
                 "http://www.example.com",
            It.IsAny<CancellationToken>()));
        
        var service = new ShortenedLinkService(mockStorage.Object);
        
        var result = await service.GenerateShortLinkAsync("http://www.example.com", CancellationToken.None);
        
        Assert.NotNull(result);
        Assert.Equal(8, result.Length);
        mockStorage.Verify(s => s.AddAsync(result,
                 "http://www.example.com",
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task CollisionThenSuccess()
    {
        var mockStorage = new Mock<IShortenedLinkStorage>();
        mockStorage.SetupSequence(s => s.AddAsync(It.IsAny<string>(),
                 It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .Throws(new ShortLinkCollisionException("Fail"))
            .Throws(new ShortLinkCollisionException("Fail"))
            .Returns(Task.CompletedTask);
        
        var service = new ShortenedLinkService(mockStorage.Object);
        
        var result = await service.GenerateShortLinkAsync("http://www.example.com", CancellationToken.None);
        
        Assert.NotNull(result);
        mockStorage.Verify(s => s.AddAsync(It.IsAny<string>(),
                 It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task CollisionThenFailure()
    {
        var mockStorage = new Mock<IShortenedLinkStorage>();
        mockStorage.Setup(s => s.AddAsync(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Throws(new ShortLinkCollisionException("Fail"));
        
        var service = new ShortenedLinkService(mockStorage.Object);
        
        await Assert.ThrowsAsync<InvalidOperationException>(() => 
            service.GenerateShortLinkAsync("http://www.example.com", CancellationToken.None));
    }

    [Fact]
    public async Task Cancelled_ThrowsOperationCanceledException()
    {
        var mockStorage = new Mock<IShortenedLinkStorage>();
        mockStorage.Setup(s => s.AddAsync(It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var cts = new CancellationTokenSource();
        cts.Cancel();
        
        var service = new ShortenedLinkService(mockStorage.Object);
        await Assert.ThrowsAsync<OperationCanceledException>(() =>
            service.GenerateShortLinkAsync("http://www.example.com",  cts.Token));
    }
}