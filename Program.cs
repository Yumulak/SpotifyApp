using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

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
        var challenge = SpotifyAuthService.GenerateCodeChallenge();
    }
}
