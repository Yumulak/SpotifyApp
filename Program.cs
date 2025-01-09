using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;
using System.Web;

class Program
{
    static void Main(string[] args)
    {
        var config = AppConfigService.InitAppConfigService();
        var spotifySettings = new SpotifyAuthSettings();
        config.GetSection("SpotifyApi").Bind(spotifySettings);
        var clientId = spotifySettings.ClientId;
        var redirectUri = spotifySettings.RedirectUri;
        var scope = spotifySettings.Scope;
        var responseType = spotifySettings.ResponseType;
        var codeChallengeMethod = spotifySettings.CodeChallengeMethod;
        string authUrl = "https://accounts.spotify.com/authorize";
        var codeVerifier = SpotifyAuthService.GenerateRandomString(64);
        var challenge = SpotifyAuthService.GenerateCodeChallenge(codeVerifier);
        Dictionary<string, string> queryParams = new Dictionary<string, string> {
            {"response_type", responseType },
            {"client_id", clientId},
            {"scope", scope},
            {"code_challenge_method", codeChallengeMethod},
            {"code_challenge", challenge},
            {"redirect_uri", redirectUri}
        };

        StoreCodeVerifier(codeVerifier);

        var uriBuilder = new UriBuilder(authUrl){
            Query = BuildQueryString(queryParams)
        };
        Console.WriteLine("Navigate to the following URL to authorize the app:");
        Console.WriteLine(uriBuilder.ToString());
    }
    static string BuildQueryString(Dictionary<string, string> parameters)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var param in parameters)
        {
            Console.WriteLine(param.Key + ": " + param.Value);
            query[param.Key] = param.Value;
        }
        return query.ToString();
    }
    static void StoreCodeVerifier(string codeVerifier)
    {
        // Store the code verifier in a secure location
        Console.WriteLine("Code verifier: " + codeVerifier);
    }
}
