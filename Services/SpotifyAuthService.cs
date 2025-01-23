static class SpotifyAuthService
{
    public static string AccessToken { get; private set; }
    public static string GenerateCodeChallenge(string? codeVerifierIn = null)
    {
        string codeVerifier;
        if (codeVerifierIn == null)
        {
            codeVerifier = GenerateRandomString(64);
        }
        else{
            codeVerifier = codeVerifierIn;
        }
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
            return Convert.ToBase64String(hash)
            .Replace("+", "-")
            .Replace("/", "_")
            .Replace("=", "");
        }
    }

    public static async Task<string> GetAccessToken(string code, string redirect_uri, string client_id, string code_verifier)
    {
        Console.WriteLine("Starting GetAccessToken");
        Console.WriteLine($"Code Verifier: {code_verifier}");        

        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("redirect_uri", redirect_uri),
            new KeyValuePair<string, string>("client_id", client_id),
            new KeyValuePair<string, string>("code_verifier", code_verifier),
        });

        var client = SpotifyAPIService.CreateHttpClient();        

        try
        {  
            var response = await client.PostAsync("https://accounts.spotify.com/api/token", requestData);      
            // response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();            
            Console.WriteLine("Status Code: " + response.StatusCode);
            Console.WriteLine("Response: " + responseString);
            var tokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(responseString);

            if(tokenResponse != null){
                AccessToken = tokenResponse.access_token;
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Error fetching access token: " + e.Message);
        }

        return AccessToken;
    }
    
}
