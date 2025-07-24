namespace UrlShortener.UnitTests;

using System;
using System.Threading;
using System.Threading.Tasks;
using Services;
using Xunit;
using Moq;

public class ShortenedLinkService_GetOriginalLinkAsync_UnitTests
{
    [Fact]
    public async Task LinkExists_ReturnOriginalLink()
    {
        var mockStorage = new Mock<IShortenedLinkStorage>();
        mockStorage.Setup(s => s.GetOriginalLinkAsync("abc123ab",
            It.IsAny<CancellationToken>()))
            .ReturnsAsync("http://www.example.com");
        
        var service = new ShortenedLinkService(mockStorage.Object);
        
        var result = await service.GetOriginalLinkAsync("abc123ab",  CancellationToken.None);
        
        Assert.Equal("http://www.example.com", result);
    }

    [Fact]
    public async Task LinkNotExists_ReturnNull()
    {
        var mockStorage = new Mock<IShortenedLinkStorage>();
        mockStorage.Setup(s => s.GetOriginalLinkAsync("invalid",
            It.IsAny<CancellationToken>()))
            .ReturnsAsync((string)null);
        
        var  service = new ShortenedLinkService(mockStorage.Object);
        
        var result = await service.GetOriginalLinkAsync("invalid", CancellationToken.None);
        
        Assert.Null(result);
    }
}