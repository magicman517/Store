using System.Security.Cryptography;
using Users.Core.Services;

namespace Users.Infrastructure.Services;

public class HashingService : IHashingService
{
    private const int SaltLength = 32;
    private const int HashLength = 32;
    private const int Iterations = 100000;

    private const string Delimiter = ":";

    private static readonly HashAlgorithmName HashAlgorithmName = HashAlgorithmName.SHA256;

    public string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltLength);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName, HashLength);

        return $"{Iterations}{Delimiter}{Convert.ToBase64String(salt)}{Delimiter}{Convert.ToBase64String(hash)}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        try
        {
            var parts = passwordHash.Split(Delimiter, 3);
            if (parts.Length != 3)
            {
                return false;
            }

            var iterations = Convert.ToInt32(parts[0]);
            var salt = Convert.FromBase64String(parts[1]);
            var hash = Convert.FromBase64String(parts[2]);

            var computedHash = Rfc2898DeriveBytes.Pbkdf2(password, salt, iterations, HashAlgorithmName, hash.Length);

            return CryptographicOperations.FixedTimeEquals(computedHash, hash);
        }
        catch (Exception)
        {
            return false;
        }
    }
}