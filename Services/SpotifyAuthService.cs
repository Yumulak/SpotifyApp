using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

static class SpotifyAuthService
{
    public static string AccessToken { get; private set; }
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

    public static async Task<string> GetAccessToken(string grant_type, string code, string redirect_uri, string client_id, string code_verifier)
    {

        var queryParams = new Dictionary<string, string> {
            {"grant_type", grant_type },
            {"code", code},
            {"redirect_uri", redirect_uri},
            {"client_id", client_id},
            {"code_verifier", code_verifier}
        };

        var content = new FormUrlEncodedContent(queryParams);
        var response = new HttpResponseMessage();

        try
        {        
            using (var client = SpotifyAPIService.CreateHttpClient()){
                response = await client.PostAsync("https://accounts.spotify.com/api/token", content);
            }

            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();
            var tokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(responseString);

            if(tokenResponse != null){
                Console.WriteLine("Access Token: " + tokenResponse.AccessToken);
                AccessToken = tokenResponse.AccessToken;
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Error fetching access token: " + e.Message);
        }

        return AccessToken;
    }
    
}
