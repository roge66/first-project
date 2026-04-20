using UrlShortener.Services;
using Moq;

namespace UrlShortener.UnitTests;

public class ShortenedLinkServiceGenerateShortLinkAsyncUnitTests
{
    private readonly Mock<IShortenedLinkStorage> _mockStorage = new Mock<IShortenedLinkStorage>();
    private readonly ShortenedLinkService _service;

    public ShortenedLinkServiceGenerateShortLinkAsyncUnitTests()
    {
        _service = new  ShortenedLinkService(_mockStorage.Object);
    }

    [Fact]
    public async Task GenerateShortLinkAsync_ShouldCreateAndSaveShortenedLink_WhenThereIsNoCollision()
    {
        // Arrange
        const string originalLink = "http://www.example.com";
        _mockStorage.Setup(s => s.AddAsync(It.IsAny<string>(), originalLink,
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GenerateShortLinkAsync(originalLink, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        Assert.Equal(8, result.Length);
        _mockStorage.Verify(s => s.AddAsync(result, originalLink,
            It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GenerateShortLinkAsync_ShouldCreateAndSaveShortenedLink_WhenThereIsCollision()
    {
        // Assert
        const string originalLink = "http://www.example.com";
        _mockStorage.SetupSequence(s => s.AddAsync(It.IsAny<string>(),
                 It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .Throws(new ShortLinkCollisionException("Fail"))
            .Throws(new ShortLinkCollisionException("Fail"))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _service.GenerateShortLinkAsync(originalLink, CancellationToken.None);
        
        // Assert
        Assert.NotNull(result);
        _mockStorage.Verify(s => s.AddAsync(It.IsAny<string>(),
                 It.IsAny<string>(),
            It.IsAny<CancellationToken>()), Times.Exactly(3));
    }

    [Fact]
    public async Task GenerateShortLinkAsync_ShouldFail_WhenThereIsCollision()
    {
        // Arrange
        const string originalLink = "http://www.example.com";
        _mockStorage.Setup(s => s.AddAsync(It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .Throws(new ShortLinkCollisionException("Fail"));

        // Act & Assert
        var act = () => _service.GenerateShortLinkAsync(originalLink, CancellationToken.None);
        await Assert.ThrowsAsync<InvalidOperationException>(act);
    }

    [Fact]
    public async Task GenerateShortLinkAsync_ShouldCancel_WhenCancellationTokenIsCanceled()
    {
        // Arrange
        const string originalLink = "http://www.example.com";
        _mockStorage.Setup(s => s.AddAsync(It.IsAny<string>(),
            It.IsAny<string>(),
            It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        
        var cts = new CancellationTokenSource();
        cts.Cancel();
        
        // Act & Assert
        var act = () => _service.GenerateShortLinkAsync(originalLink,  cts.Token);
        await Assert.ThrowsAsync<OperationCanceledException>(act);
    }
}