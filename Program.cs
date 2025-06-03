
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
        Console.WriteLine();

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
        var accessToken = SpotifyAuthService.GetAccessToken(code, redirectUri, clientId, codeVerifier);
        // Console.WriteLine("Access Token in Main: " + accessToken.Result.Item1);
        // Console.WriteLine("Refresh Token in Main: " + accessToken.Result.Item2);        
        
        //store the access token securely
        string encryptionKey = "Your32CharLongEncryptionKey!";
        var keyVault = new SimulatedKeyVaultService(encryptionKey);
        keyVault.StoreSecret("access_token", accessToken.Result.Item1);
        keyVault.StoreSecret("refresh_token", accessToken.Result.Item2);

        var retrievedAccessToken = keyVault.RetrieveSecret("access_token");
        var retrievedRefreshToken = keyVault.RetrieveSecret("refresh_token");

        //use access token to make requests to spotify API (get user profile, playlists, etc.)
        
        Console.WriteLine("\nFetching user profile...");
        var userProfile = SpotifyAPIService.GetUserProfile(retrievedAccessToken);
        // Console.WriteLine($"Retrieved access token: {retrievedAccessToken}");
        Console.WriteLine("Fetched user profile for: " + userProfile.Result.display_name + " Email: " + userProfile.Result.email);
        var likedSongs = SpotifyAPIService.GetAllUsersLikedSongs(retrievedAccessToken);

        bool stop = false;
        while(stop == false){
            Console.WriteLine("Get liked songs? s");
            Console.WriteLine("Get playlists? p");
            Console.WriteLine("Get genres? g");
            Console.WriteLine("Create new playlist by genre? npl");
            Console.WriteLine("Exit? e");
            string yesNo = Console.ReadLine();
            if (yesNo == "s")
            {
                Console.WriteLine("Fetching user liked songs...");
                foreach (var item in likedSongs.Result.Item1)
                {
                    Console.WriteLine(item);
                }
            }
            else if (yesNo == "p")
            {
                Console.WriteLine("Fetching user playlists...");
                var playLists = SpotifyAPIService.GetAllUsersPlaylists(retrievedAccessToken);
                foreach (var item in playLists.Result)
                {
                    Console.WriteLine(item);
                }
            }
            //list in GetAllUsersLikedSongs will have duplicate songs, causing the SongsGenre dictionary to have duplicate keys
            else if (yesNo == "g")
            {
                Console.WriteLine("Fetching genres...");
                var genres = SpotifyAPIService.GetSongGenresDict(retrievedAccessToken, likedSongs.Result.Item2);
                foreach (var genre in genres.Result.Item2)
                {
                    Console.WriteLine(genre);
                }
            }
            else if (yesNo == "npl")
            {
                Console.WriteLine("What genre would you like to create a playlist for?");
                string genre = Console.ReadLine();
                Console.WriteLine($"Creating new playlist for {genre} genre...");
                //call create playlist method in SpotifyAPIService and pass in the genre
                var createdPlaylist = SpotifyAPIService.CreatePlaylist(retrievedAccessToken, userProfile.Result.id, genre);
                Console.WriteLine($"New playlist created for {genre} genre.");
                Console.WriteLine("Created playlist at this url: " + createdPlaylist.Result);
            }
            else if (yesNo == "e")
            {
                Console.WriteLine("Exiting...");
                stop = true;
            }
        }

    }
    static string BuildQueryString(Dictionary<string, string> parameters)
    {
        var query = HttpUtility.ParseQueryString(string.Empty);
        foreach (var param in parameters)
        {
            // Console.WriteLine(param.Key + ": " + param.Value); uncomment to debug
            query[param.Key] = param.Value;
        }
        return query.ToString();
    }
    static void StoreCodeVerifier(string codeVerifier)
    {
        // Store the code verifier in a secure location
        // Console.WriteLine("Code verifier in main file: " + codeVerifier);
    }
}
