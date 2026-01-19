using System.Text.Json;

using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

using SmartCommune.Application.Common.Interfaces.Services;

namespace SmartCommune.Infrastructure.Services;

public class CacheService(
    IDistributedCache distributedCache,
    ILogger<CacheService> logger) : ICacheService
{
    // --- KHAI BÁO BIẾN CHO CIRCUIT BREAKER ---
    private const int CircuitBreakerDurationSeconds = 60; // Ngắt cầu dao trong 60 giây.
    private static bool _isRedisDown = false;
    private static DateTime _nextRetryTime = DateTime.MinValue;
    private readonly Lock _lock = new();

    private readonly IDistributedCache _distributedCache = distributedCache;
    private readonly ILogger<CacheService> _logger = logger;

    public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default)
    {
        // 1. KIỂM TRA CẦU DAO
        // Nếu Redis đang bị đánh dấu là chết VÀ chưa đến giờ thử lại
        // -> Return luôn, không thèm gọi distributedCache.GetStringAsync => không phải đợi hết timeout.
        if (_isRedisDown && DateTime.UtcNow < _nextRetryTime)
        {
            return default;
        }

        try
        {
            string? cachedValue = await _distributedCache.GetStringAsync(key, cancellationToken);

            // Nếu gọi thành công -> Reset trạng thái Redis sống lại.
            if (_isRedisDown)
            {
                lock (_lock)
                {
                    _isRedisDown = false;
                    _logger.LogInformation("Redis is back online!");
                }
            }

            return string.IsNullOrEmpty(cachedValue)
                ? default
                : JsonSerializer.Deserialize<T>(cachedValue);
        }
        catch (Exception ex)
        {
            HandleRedisException(ex, "GET", key);
            return default;
        }
    }

    public async Task SetAsync<T>(string key, T value, TimeSpan? expiration = null, CancellationToken cancellationToken = default)
    {
        if (_isRedisDown && DateTime.UtcNow < _nextRetryTime)
        {
            return;
        }

        try
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration ?? TimeSpan.FromMinutes(60),
            };
            string cacheValue = JsonSerializer.Serialize(value);

            await _distributedCache.SetStringAsync(key, cacheValue, options, cancellationToken);

            if (_isRedisDown)
            {
                lock (_lock)
                {
                    _isRedisDown = false;
                    _logger.LogInformation("Redis is back online!");
                }
            }
        }
        catch (Exception ex)
        {
            HandleRedisException(ex, "SET", key);
        }
    }

    public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
    {
        if (_isRedisDown && DateTime.UtcNow < _nextRetryTime)
        {
            return;
        }

        try
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);

            if (_isRedisDown)
            {
                lock (_lock)
                {
                    _isRedisDown = false;
                    _logger.LogInformation("Redis is back online!");
                }
            }
        }
        catch (Exception ex)
        {
            HandleRedisException(ex, "REMOVE", key);
        }
    }

    // Hàm xử lý lỗi chung để kích hoạt ngắt cầu dao.
    private void HandleRedisException(Exception ex, string action, string key)
    {
        _logger.LogError(
            ex,
            "Redis connection failed when {Action} key {Key}. Circuit Breaker OPEN for {Seconds}s.",
            action,
            key,
            CircuitBreakerDurationSeconds);

        // KÍCH HOẠT NGẮT CẦU DAO.
        lock (_lock)
        {
            _isRedisDown = true;
            _nextRetryTime = DateTime.UtcNow.AddSeconds(CircuitBreakerDurationSeconds);
        }
    }
}