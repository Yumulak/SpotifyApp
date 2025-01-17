using System;
using System.Security.Cryptography;
using System.Text;

static class SpotifyAuthService
{
    
    public static string GenerateCodeChallenge(string? codeVerifierIn = null)
    {
        var codeVerifier = "";
        if (codeVerifierIn == null)
        {
            codeVerifier = GenerateRandomString(64);
        }
        else{
            codeVerifier = codeVerifierIn;
        }
        Console.WriteLine($"Code Verifier in GenerateCodeChallenge: {codeVerifier}");
        var codeChallenge = GenerateSha256(codeVerifier);
        return codeChallenge;
    }
    public static string GenerateRandomString(int length)
    {
        const string possible = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var randomBytes = new byte[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }

        var stringBuilder = new StringBuilder(length);
        foreach (var b in randomBytes)
        {
            stringBuilder.Append(possible[b % possible.Length]);
        }

        return stringBuilder.ToString();
    }

    static string GenerateSha256(string codeVerifier)
    {
        using (var sha256 = SHA256.Create())
        {
            var bytes = Encoding.UTF8.GetBytes(codeVerifier);
            var hash = sha256.ComputeHash(bytes);
            return Convert.ToBase64String(hash);
        }
    }
}
