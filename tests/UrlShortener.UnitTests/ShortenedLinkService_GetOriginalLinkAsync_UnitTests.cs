using UrlShortener.Services;
using Moq;

namespace UrlShortener.UnitTests;
public class ShortenedLinkServiceGetOriginalLinkAsyncUnitTests
{
    private readonly Mock<IShortenedLinkStorage> _mockStorage = new Mock<IShortenedLinkStorage>();
    private readonly ShortenedLinkService _service;

    public ShortenedLinkServiceGetOriginalLinkAsyncUnitTests()
    {
        _service = new  ShortenedLinkService(_mockStorage.Object);
    }
    [Fact]
    public async Task GetOriginalLinkAsync_ShouldReturnOriginalLink_WhenLinkExists()
    {
        // Arrange
        const string originalLink = "http://www.example.com";
        const string shortCode = "abc123ab";
        _mockStorage.Setup(s => s.GetOriginalLinkAsync(shortCode,
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(originalLink);
        
        // Act
        var result = await _service.GetOriginalLinkAsync(shortCode,  CancellationToken.None);
        
        // Assert
        Assert.Equal(originalLink, result);
    }

    [Fact]
    public async Task GetOriginalLinkAsync_ShouldReturnNull_WhenLinkNotExists()
    {
        // Arrange
        _mockStorage.Setup(s => s.GetOriginalLinkAsync("invalid",
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null);
        
        // Act
        var result = await _service.GetOriginalLinkAsync("invalid", CancellationToken.None);
        
        // Assert
        Assert.Null(result);
    }
}