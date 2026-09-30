using Labirint.Core.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Labirint.Web.Common.Seeding;

public sealed class SeedSource : IRandom
{
    private Random? _random;

    public Random Generator => _random ?? Random.Shared;

    public int CurrentSeed { get; private set; }

    public string? UserSeed { get; set; }

    public bool IsGenerateRequired => CurrentSeed < 0;

    public static int GenerateSeed(string input)
    {
        var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        var result = BitConverter.ToInt32(hashBytes, 0);

        return result == int.MinValue ? int.MaxValue : Math.Abs(result);
    }

    public static int ParseSeed(string userSeed)
    {
        return userSeed.All(char.IsDigit) && int.TryParse(userSeed, out var parsedSeed)
            ? parsedSeed
            : GenerateSeed(userSeed);
    }

    public void Repeat()
    {
        if (IsGenerateRequired)
        {
            Reload();
            return;
        }

        _random = new(CurrentSeed);
    }

    public void Reload(bool force = false)
    {
        if (force || string.IsNullOrWhiteSpace(UserSeed))
        {
            ReloadWithRandomSeed();
            return;
        }

        CurrentSeed = ParseSeed(UserSeed);
        _random = new(CurrentSeed);
    }

    public void ResetSeed()
    {
        CurrentSeed = -1;
    }

    private void ReloadWithRandomSeed()
    {
        UserSeed = null;
        CurrentSeed = Random.Shared.Next();
        _random = new(CurrentSeed);
    }
}
