using Xunit;
using Moq;
using Moq.Protected;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using Tau_CoinDesk_Api.Services;


public class CoinDeskServiceTests
{
    private HttpClient CreateHttpClient(HttpResponseMessage response)
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ReturnsAsync(response);

        return new HttpClient(handlerMock.Object);
    }

    [Fact]
    public async Task GetRatesAsync_ReturnsJson_WhenApiOk()
    {
        // Arrange
        var json = "{\"test\":123}";
        var response = new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(json, Encoding.UTF8, "application/json")
        };

        var client = CreateHttpClient(response);
        var logger = Mock.Of<ILogger<CoinDeskService>>();

        var service = new CoinDeskService(client, logger);

        // Act
        var result = await service.GetRatesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.Equal(123, result.RootElement.GetProperty("test").GetInt32());
    }

    [Fact]
    public async Task GetRatesAsync_ReturnsMockData_WhenApiError()
    {
        // Arrange
        var response = new HttpResponseMessage(HttpStatusCode.Forbidden); // 403
        var client = CreateHttpClient(response);
        var logger = Mock.Of<ILogger<CoinDeskService>>();

        var service = new CoinDeskService(client, logger);

        // Act
        var result = await service.GetRatesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.RootElement.GetProperty("isMock").GetBoolean());
    }

    [Fact]
    public async Task GetRatesAsync_ReturnsMockData_WhenHttpRequestException()
    {
        // Arrange: 用拋出例外的 handler
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            .ThrowsAsync(new HttpRequestException("DNS fail"));

        var client = new HttpClient(handlerMock.Object);
        var logger = Mock.Of<ILogger<CoinDeskService>>();

        var service = new CoinDeskService(client, logger);

        // Act
        var result = await service.GetRatesAsync();

        // Assert
        Assert.NotNull(result);
        Assert.True(result.RootElement.GetProperty("isMock").GetBoolean());
    }
}
