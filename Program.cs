using System.IO.Pipes;

class Program
{
    static async Task Main(string[] args)
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

        try
        {
            Log.Information("Application starting...");
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An unhandled exception occurred during application startup.");
        }
        finally
        {
            Log.CloseAndFlush();
        }
        
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
        var songGenres = SpotifyAPIService.GetSongGenresDict(retrievedAccessToken, likedSongs.Result.Item2);
        var songsGenresDict = songGenres.Result.Item1;
        var songsGenresList = songGenres.Result.Item2;
        var playLists = SpotifyAPIService.GetAllUsersPlaylists(retrievedAccessToken);
        Console.WriteLine("Loading...");
        
        PrintConsoleMessage();
        bool stop = false;
        while (stop == false)
        {

            string yesNo = Console.ReadLine();
            if (yesNo == "s")
            {
                Console.WriteLine("Fetching user liked songs...");
                foreach (var item in likedSongs.Result.Item1)
                {
                    Console.WriteLine(item);
                }
                PrintConsoleMessage();
            }
            else if (yesNo == "p")
            {
                Console.WriteLine("Fetching user playlists...");
                foreach (var item in playLists.Result)
                {
                    Console.WriteLine(item);
                }
                PrintConsoleMessage();
            }
            //list in GetAllUsersLikedSongs will have duplicate songs, causing the SongsGenre dictionary to have duplicate keys
            else if (yesNo == "g")
            {
                Console.WriteLine("Fetching genres...");
                foreach (var genre in songsGenresList)
                {
                    Console.WriteLine(genre);
                }
                PrintConsoleMessage();
            }
            else if (yesNo == "npl")
            {
                Console.WriteLine("Available genres:");
                foreach (var genre in songsGenresList)
                {
                    Console.WriteLine(genre);
                }
                Console.WriteLine("What genre would you like to create a playlist for?");
                string song_genre = "";
                while (true)
                {
                    Console.WriteLine("Please enter a genre from the list above:");
                    song_genre = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(song_genre))
                    {
                        Console.WriteLine("Input cannot be empty. Please try again.");
                        continue;
                    }
                    if (!songsGenresList.Contains(song_genre))
                    {
                        Console.WriteLine("Genre not found. Please try again.");
                        continue;
                    }
                    break;
                }

                Console.WriteLine($"Creating new playlist for {song_genre} genre...");
                //call create playlist method in SpotifyAPIService and pass in the genre                
                var createdPlaylist = SpotifyAPIService.CreatePlaylist(retrievedAccessToken, userProfile.Result.id, song_genre);
                List<string> songsUrisToAdd = likedSongs.Result.Item2.Where(song => songsGenresDict.ContainsKey(song.track.name) && songsGenresDict[song.track.name].Contains(song_genre))
                    .Select(song => song.track.uri).ToList();

                //put all songs in the song_genres dictionary into the playlist that have the input genre in the values list
                if (songsUrisToAdd.Count > 0)
                {
                    Console.WriteLine($"Adding {songsUrisToAdd.Count} songs to the playlist...");
                    SpotifyAPIService.AddSongsToPlaylist(retrievedAccessToken, createdPlaylist.Result.id, songsUrisToAdd);
                }
                else
                {
                    Console.WriteLine("No songs found for the specified genre.");
                }
                Console.WriteLine($"New playlist created for {song_genre} genre with {songsUrisToAdd.Count} songs.");
                Console.WriteLine("Created playlist at this url: " + createdPlaylist.Result.href);
                PrintConsoleMessage();
            }
            else if (yesNo == "e")
            {
                Console.WriteLine("Exiting...");
                stop = true;
            }
            else
            {
                Console.WriteLine("Invalid input. Please try again.");
                PrintConsoleMessage();
            }
        }

    }
    public static void PrintConsoleMessage()
    {
        Console.WriteLine("\nGet liked songs? s");
        Console.WriteLine("Get playlists? p");
        Console.WriteLine("Get genres? g");
        Console.WriteLine("Create new playlist by genre? npl");
        Console.WriteLine("Exit? e");
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
