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
        //load configuration from appsettings.json
        var config = AppConfigService.InitAppConfigService();
        var spotifySettings = new SpotifyAuthSettings();
        config.GetSection("SpotifyApi").Bind(spotifySettings);
        var clientId = spotifySettings.ClientId;
        var redirectUri = spotifySettings.RedirectUri;
        var scope = spotifySettings.Scope;
        var responseType = spotifySettings.ResponseType;
        var codeChallengeMethod = spotifySettings.CodeChallengeMethod;
        var SpotifyBaseUrl = spotifySettings.SpotifyApiBaseUrl;
        string authUrl = $"{SpotifyBaseUrl}authorize";

        //generate code verifier and challenge
        var codeVerifier = SpotifyAuthService.GenerateRandomString(64);
        var challenge = SpotifyAuthService.GenerateCodeChallenge(codeVerifier);

        //create query parameters for authorization URL
        Dictionary<string, string> queryParams = new Dictionary<string, string> {
            {"response_type", responseType },
            {"client_id", clientId},
            {"scope", scope},
            {"code_challenge_method", codeChallengeMethod},
            {"code_challenge", challenge},
            {"redirect_uri", redirectUri}
        };

        //store the code verifier for later use
        StoreCodeVerifier(codeVerifier);
        
        //build the authorization URL with query parameters from config
        var uriBuilder = new UriBuilder(authUrl){
            Query = BuildQueryString(queryParams)
        };
        Console.WriteLine("Navigating to the following URL to authorize the app:");
        Console.WriteLine(uriBuilder.ToString());

        //open the browser to the authorization url
        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
        {
            FileName = uriBuilder.ToString(), 
            UseShellExecute = true
        });
        
        //wait for the callback from spotify and retrieve the authorization code from 
        var code = "";
        try{
            code = WaitForSpotifyCallback.WaitForCallback();
            Console.WriteLine($"Authorization code received: {code}");
        }
        catch(Exception e){
            Console.WriteLine("Error: " + e.Message);
            
        }
        

        //use authorization code to request access token
        //send post request to https://accounts.spotify.com/api/token
        var accessToken = SpotifyAuthService.GetAccessToken("authorization_code", code, redirectUri, clientId, codeVerifier);
        Console.WriteLine("Access Token: " + accessToken.ToString());
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
        Console.WriteLine("Code verifier in main file: " + codeVerifier);
    }
}
