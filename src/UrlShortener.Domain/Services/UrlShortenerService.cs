using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace UrlShortener.Domain.Services;

public class UrlShortenerService : IUrlShortenerService
{
    private const int DefaultLength = 6;
    
    public string GenerateShortUrl(string url, int numberOfEntries)
    {
        if (string.IsNullOrEmpty(url))
        {
            throw new ArgumentNullException(nameof(url));
        }

        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(url));

        var base62Hash = ConvertToBase62(hashBytes);

        return base62Hash[..Math.Min(CalculateHashLengthBasedOnEntries(numberOfEntries), base62Hash.Length)];
    }

    private static string ConvertToBase62(byte[] bytes)
    {
        const string base62Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        Array.Resize(ref bytes, bytes.Length + 1);
        
        var number = new BigInteger(bytes);
        var sb = new StringBuilder();

        while (number > 0)
        {
            var remainder = (int)(number % 62);
            sb.Insert(0, base62Chars[remainder]);
            number /= 62;
        }

        return sb.ToString();
    }
    
    private static int CalculateHashLengthBasedOnEntries(int numberOfEntries) 
        => DefaultLength + (numberOfEntries / 10000);
}