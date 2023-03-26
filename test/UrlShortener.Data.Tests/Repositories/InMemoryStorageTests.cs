using FluentAssertions.Execution;
using UrlShortener.Data.Repositories;
using UrlShortener.Domain.Entities;

namespace UrlShortener.Data.Tests.Repositories;

    public class InMemoryStorageTests
    {
        private readonly InMemoryStorage _storage;

        public InMemoryStorageTests()
        {
            _storage = new InMemoryStorage();
        }

        [Fact]
        public async Task AddAsync_WhenShortUrlAdded_ReturnsTrue()
        {
            var shortUrl = new ShortUrl("short", "https://example.com");

            var result = await _storage.AddAsync(shortUrl);

            result.Should().BeTrue();
        }

        [Fact]
        public async Task GetAsync_WhenShortUrlExists_ReturnsUrl()
        {
            var shortUrl = new ShortUrl("short", "https://example.com");
            await _storage.AddAsync(shortUrl);

            var result = await _storage.GetAsync("short");

            result.Should().Be("https://example.com");
        }

        [Fact]
        public async Task GetAsync_WhenShortUrlNotExists_ReturnsNull()
        {
            var result = await _storage.GetAsync("nonexistent");

            result.Should().BeNull();
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnsAllUrls()
        {
            var shortUrl1 = new ShortUrl("short1", "https://example.com/1");
            var shortUrl2 = new ShortUrl("short2", "https://example.com/2");
            await _storage.AddAsync(shortUrl1);
            await _storage.AddAsync(shortUrl2);

            var results = (await _storage.GetAllAsync(0, 0)).ToList();

            using (new AssertionScope())
            {
                results.Should().HaveCount(2);
                results.Should().Contain(shortUrl1);
                results.Should().Contain(shortUrl2);
            }
        }

        [Fact]
        public async Task GetAllAsync_WhenPaging_ReturnsPagedUrls()
        {
            var shortUrl1 = new ShortUrl("short1", "https://example.com/1");
            var shortUrl2 = new ShortUrl("short2", "https://example.com/2");
            await _storage.AddAsync(shortUrl1);
            await _storage.AddAsync(shortUrl2);

            var results = (await _storage.GetAllAsync(0, 1)).ToList();

            using (new AssertionScope())
            {
                results.Should().HaveCount(1);
                results.Should().Contain(shortUrl1);
            }
        }

        [Fact]
        public async Task GetNumberOfEntriesAsync_WhenCalled_ReturnsNumberOfEntries()
        {
            var shortUrl1 = new ShortUrl("short1", "https://example.com/1");
            var shortUrl2 = new ShortUrl("short2", "https://example.com/2");
            await _storage.AddAsync(shortUrl1);
            await _storage.AddAsync(shortUrl2);

            var count = await _storage.GetNumberOfEntriesAsync();

            count.Should().Be(2);
        }
    }
