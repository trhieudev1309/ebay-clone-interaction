using System.Text.Json;
using EbayChat.Entities;
using StackExchange.Redis;

namespace EbayChat.Services.ServicesImpl
{
    public class RedisCacheService : IRedisCacheService
    {
        private readonly IDatabase _database;
        private readonly ILogger<RedisCacheService> _logger;
        private readonly IConfiguration _configuration;
        private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        public RedisCacheService(
            IConnectionMultiplexer multiplexer,
            ILogger<RedisCacheService> logger,
            IConfiguration configuration)
        {
            _database = multiplexer.GetDatabase();
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<List<Message>> GetMessagesAsync(string conversationKey)
        {
            var raw = await _database.StringGetAsync(conversationKey);

            if (!raw.HasValue)
            {
                _logger.LogInformation("Redis cache miss for key: {ConversationKey}", conversationKey);
                return new List<Message>();
            }

            try
            {
                var messages = JsonSerializer.Deserialize<List<Message>>(raw!, _jsonOptions) ?? new List<Message>();
                _logger.LogInformation(
                    "Redis cache hit for key: {ConversationKey}, Count: {Count}",
                    conversationKey,
                    messages.Count);

                return messages;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to deserialize cached messages for key: {ConversationKey}", conversationKey);
                return new List<Message>();
            }
        }

        public async Task SetMessagesAsync(string conversationKey, List<Message> messages, TimeSpan? expiry = null)
        {
            // Tradeoff:
            // - Redis String(JSON): simple full-conversation read/write (chosen here).
            // - Redis List (LPUSH/LRANGE): more efficient append, but more complex for reorder/update.
            var defaultMinutes = _configuration.GetValue<int?>("Redis:DefaultTtlMinutes") ?? 10;
            var ttl = expiry ?? TimeSpan.FromMinutes(defaultMinutes);

            var payload = JsonSerializer.Serialize(messages, _jsonOptions);
            await _database.StringSetAsync(conversationKey, payload, ttl);

            _logger.LogInformation(
                "Redis cache set for key: {ConversationKey}, Count: {Count}, TTL(min): {Ttl}",
                conversationKey,
                messages.Count,
                ttl.TotalMinutes);
        }

        public async Task InvalidateMessagesAsync(string conversationKey)
        {
            await _database.KeyDeleteAsync(conversationKey);
            _logger.LogInformation("Redis cache invalidated for key: {ConversationKey}", conversationKey);
        }
    }
}