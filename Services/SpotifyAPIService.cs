using System.Web;

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
}