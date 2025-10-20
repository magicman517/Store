using System.Security.Cryptography;
using System.Text;
using Users.Core.Entities;

namespace Users.Application.DTOs.Users;

public record UserResponse
{
    public Guid Id { get; init; }
    public required string Email { get; init; }

    public required string AvatarUrl { get; init; }

    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public string? MiddleName { get; init; }

    public string? PhoneNumber { get; init; }

    public IList<string> Roles { get; init; } = [];
    public IList<string> LinkedAccounts { get; init; } = [];

    public static UserResponse FromEntity(User user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        AvatarUrl = GetGravatarUrl(user.Email),
        FirstName = user.FirstName,
        LastName = user.LastName,
        MiddleName = user.MiddleName,
        PhoneNumber = user.PhoneNumber,
        Roles = user.Roles,
        LinkedAccounts = user.LinkedAccounts.Select(la => la.Provider.ToString()).ToList()
    };

    private static string GetGravatarUrl(string email)
    {
        var trimmedEmail = email.Trim().ToLowerInvariant();
        var emailBytes = Encoding.UTF8.GetBytes(trimmedEmail);
        var hashBytes = MD5.HashData(emailBytes);

        var sb = new StringBuilder();
        foreach (var b in hashBytes)
        {
            sb.Append(b.ToString("x2"));
        }

        var hashString = sb.ToString();

        return $"https://www.gravatar.com/avatar/{hashString}?s=80&d=identicon"; // available options: mp, identicon, monsterid, wavatar, retro, robohash, 404
    }
}