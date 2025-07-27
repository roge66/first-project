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
    public async Task GetOriginalLinkAsync_LinkExists_ReturnOriginalLink()
    {
        // Arrange
        _mockStorage.Setup(s => s.GetOriginalLinkAsync("abc123ab",
            It.IsAny<CancellationToken>()))
            .ReturnsAsync("http://www.example.com");
        
        // Act
        var result = await _service.GetOriginalLinkAsync("abc123ab",  CancellationToken.None);
        
        // Assert
        Assert.Equal("http://www.example.com", result);
    }

    [Fact]
    public async Task GetOriginalLinkAsync_LinkNotExists_ReturnNull()
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