using FluentAssertions.Execution;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using UrlShortener.Data.Redis;
using UrlShortener.Data.Repositories;
using UrlShortener.Domain.Entities;
using UrlShortener.Domain.Repositories;

namespace UrlShortener.Data.Tests.Repositories;

public class RedisShortUrlRepositoryTests
{
    private readonly Mock<IDatabase> _databaseMock;
    private readonly Mock<IStatisticsRepository> _statisticsRepositoryMock;
    private readonly RedisShortUrlRepository _repository;

    public RedisShortUrlRepositoryTests()
    {
        var connectionMultiplexerMock = new Mock<IConnectionMultiplexer>();
        var subscriberMock = new Mock<ISubscriber>();
        var loggerMock = new Mock<ILogger<RedisShortUrlRepository>>();
        
        _databaseMock = new Mock<IDatabase>();
        _statisticsRepositoryMock = new Mock<IStatisticsRepository>();

        connectionMultiplexerMock.Setup(multiplexer => multiplexer.GetDatabase(It.IsAny<int>(), It.IsAny<object>()))
            .Returns(_databaseMock.Object);

        connectionMultiplexerMock.Setup(multiplexer => multiplexer.GetSubscriber(null))
            .Returns(subscriberMock.Object);
        
        var configuration = new RedisConfiguration
        {
            TimeToLive = 10
        };

        _repository = new RedisShortUrlRepository(connectionMultiplexerMock.Object, configuration,
            _statisticsRepositoryMock.Object, loggerMock.Object);
    }

    [Fact]
    public async Task AddAsync_ReturnsAddShortUrlToRedisDatabase()
    {
        var shortUrl = new ShortUrl("short", "https://example.com");
        
        _databaseMock.Setup(db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), false, When.Always, CommandFlags.None)).ReturnsAsync(true);
        _databaseMock.Setup(db => db.SortedSetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<double>(), SortedSetWhen.Always, CommandFlags.None)).ReturnsAsync(true);
        _statisticsRepositoryMock.Setup(repo => repo.GetScoreForKey(It.IsAny<string>(), It.IsAny<string>()))
            .ReturnsAsync(1);

        var result = await _repository.AddAsync(shortUrl);


        using (new AssertionScope())
        {
            result.Should().BeTrue();
            _databaseMock.Verify(
                db => db.StringSetAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<TimeSpan>(), false, When.Always,
                    CommandFlags.None), Times.Once);
            _databaseMock.Verify(
                db => db.SortedSetAddAsync(It.IsAny<RedisKey>(), It.IsAny<RedisValue>(), It.IsAny<double>(), SortedSetWhen.Always,
                    CommandFlags.None), Times.Once);
            _statisticsRepositoryMock.Verify(m => m.IncrementCounterAsync(It.IsAny<string>()), Times.Once);
        }
    }

    [Fact]
    public async Task GetAsync_WhenShortUrlExists_ReturnsOriginalUrl()
    {
        const string shortUrl = "short";
        const string originalUrl = "https://example.com";
        _databaseMock.Setup(db => db.StringGetAsync(shortUrl, CommandFlags.None))
            .ReturnsAsync(new RedisValue(originalUrl));

        var result = await _repository.GetAsync(shortUrl);

        using (new AssertionScope())
        {
            result.Should().NotBeNull();
            result.Should().Be(originalUrl);
        }
    }

    [Fact]
    public async Task GetAsync_WhenShortUrlDoesNotExist_ReturnsNull()
    {
        const string shortUrl = "nonexistent";
        
        _databaseMock.Setup(db => db.StringGetAsync(shortUrl, CommandFlags.None)).ReturnsAsync(new RedisValue());

        var result = await _repository.GetAsync(shortUrl);

        result.Should().BeNull();
    }

    [Fact]
    public async Task GetNumberOfEntriesAsync_ReturnsEntriesCount()
    {
        _statisticsRepositoryMock.Setup(repo => repo.GetNumberOfEntriesAsync(It.IsAny<string>())).ReturnsAsync(100);

        var result = await _repository.GetNumberOfEntriesAsync();

        result.Should().Be(100);
    }
}