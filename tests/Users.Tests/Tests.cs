namespace Users.Tests;

public class Tests(AspireAppHostFixture fixture) : IClassFixture<AspireAppHostFixture>
{
    private static readonly TimeSpan DefaultTimeout = TimeSpan.FromSeconds(120);
}