using StackExchange.Redis;
using UrlShortener.Data.Repositories;

namespace UrlShortener.Data.Tests.Repositories;

public class RedisStatisticsRepositoryTests
    {
        private readonly Mock<IDatabase> _databaseMock;
        private readonly RedisStatisticsRepository _repository;

        public RedisStatisticsRepositoryTests()
        {
            var connectionMultiplexerMock = new  Mock<IConnectionMultiplexer>();
            _databaseMock = new Mock<IDatabase>();
            connectionMultiplexerMock.Setup(m => m.GetDatabase(It.IsAny<int>(), It.IsAny<object>())).Returns(_databaseMock.Object);
            _repository = new RedisStatisticsRepository(connectionMultiplexerMock.Object);
        }

        [Fact]
        public async Task GetScoreForKey_WhenCalled_ReturnsScoreForKey()
        {
            const string sortedSetName = "sortedSet";
            const string key = "key";
            _databaseMock.Setup(db => db.SortedSetScoreAsync(sortedSetName, key, CommandFlags.None)).ReturnsAsync((double?)1);

            var score = await _repository.GetScoreForKey(sortedSetName, key);

            score.Should().Be(2);
        }

        [Fact]
        public async Task IncrementAccessCountAsync_WhenCalled_IncrementsAccessCount()
        {
            const string sortedSetName = "sortedSet";
            const string key = "key";
            _databaseMock.Setup(db => db.SortedSetIncrementAsync(sortedSetName, key, 1, CommandFlags.None)).ReturnsAsync(1d);

            await _repository.IncrementAccessCountAsync(sortedSetName, key);

            _databaseMock.Verify(db => db.SortedSetIncrementAsync(sortedSetName, key, 1, CommandFlags.None), Times.Once);
        }

        [Fact]
        public async Task GetNumberOfEntriesAsync_WhenCalled_ReturnsNumberOfEntries()
        {
            const string key = "key";
            _databaseMock.Setup(db => db.StringGetAsync(key, CommandFlags.None)).ReturnsAsync((RedisValue)1);

            var numberOfEntries = await _repository.GetNumberOfEntriesAsync(key);

            numberOfEntries.Should().Be(1);
        }

        [Fact]
        public async Task IncrementCounterAsync_WhenCalled_IncrementsCounter()
        {
            const string counterKey = "counter";
            _databaseMock.Setup(db => db.StringIncrementAsync(counterKey, 1L, CommandFlags.None)).ReturnsAsync(1L);

            await _repository.IncrementCounterAsync(counterKey);

            _databaseMock.Verify(db => db.StringIncrementAsync(counterKey, 1L, CommandFlags.None), Times.Once);
        }
    }