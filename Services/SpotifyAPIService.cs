public class SpotifyAPIService
{
    public static string BuildQueryString(Dictionary<string, string> parameters)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var param in parameters)
        {
            Console.WriteLine(param.Key + ": " + param.Value);
            query[param.Key] = param.Value;
        }
        return query.ToString();
    }

    public static HttpClient CreateHttpClient(string? accessToken = null){
        
        var client = new HttpClient();
        // client.DefaultRequestHeaders.Add("Authorization", "Bearer " + accessToken);
        return client;
    }

    //GetUserProfile to be implemented to get user profile information
    //currently code copied from GetAccessToken
    public static async Task GetUserProfile(string code, string redirect_uri, string client_id, string code_verifier)
    {
        Console.WriteLine("Starting GetUserProfile");

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
            var response = await client.PostAsync("https://accounts.spotify.com/v1/me", requestData);      
            response.EnsureSuccessStatusCode();

            var responseString = await response.Content.ReadAsStringAsync();            
            Console.WriteLine("Status Code: " + response.StatusCode);
            Console.WriteLine("Response: " + responseString);
            var tokenResponse = JsonSerializer.Deserialize<AccessTokenResponse>(responseString);

            if(tokenResponse != null){
                
            }
        }
        catch(Exception e)
        {
            Console.WriteLine("Error fetching access token: " + e.Message);
        }

        
    }
}