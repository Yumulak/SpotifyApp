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
    public static HttpClient CreateHttpClient(string? accessToken = null)
    {

        var client = new HttpClient();
        if (accessToken != null)
        {
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        }
        return client;
    }
    public static async Task<UserProfile> GetUserProfile(string accessToken)
    {
        // Console.WriteLine("Starting GetUserProfile");

        var client = new HttpClient();
        var request = new HttpRequestMessage();
        request.RequestUri = new Uri("https://api.spotify.com/v1/me");
        request.Method = HttpMethod.Get;

        request.Headers.Add("Authorization", "Bearer " + accessToken);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        var userProfile = JsonSerializer.Deserialize<UserProfile>(result);
        return userProfile;
    }
    private static async Task<LikedSongs> GetPageOfUsersLikedSongs(string accessToken, string? url = null)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage();
        if (url != null)
        {
            request.RequestUri = new Uri(url);
        }
        else
        {
            request.RequestUri = new Uri("https://api.spotify.com/v1/me/tracks?limit=50");
        }
        request.Method = HttpMethod.Get;

        request.Headers.Add("Authorization", "Bearer " + accessToken);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        var likedSongs = JsonSerializer.Deserialize<LikedSongs>(result);
        return likedSongs;
    }
    public static async Task<(List<string>, HashSet<Models.LikedSongs.Item>)> GetAllUsersLikedSongs(string accessToken)
    {
        List<string> allSongs = new List<string>();
        HashSet<Models.LikedSongs.Item> allLikedSongObject = new HashSet<Models.LikedSongs.Item>();
        var currentPage = GetPageOfUsersLikedSongs(accessToken);
        allLikedSongObject.UnionWith(currentPage.Result.items);
        allSongs.AddRange(currentPage.Result.items.Select(item => item.track.name));
        while (currentPage.Result.next != null)
        {
            currentPage = GetPageOfUsersLikedSongs(accessToken, currentPage.Result.next);
            allSongs.AddRange(currentPage.Result.items.Select(item => item.track.name));
            allLikedSongObject.UnionWith(currentPage.Result.items);
        }
        return (allSongs, allLikedSongObject);
    }
    private static async Task<Playlists> GetPageOfUsersPlaylists(string accessToken, string? url = null)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage();
        if (url != null)
        {
            request.RequestUri = new Uri(url);
        }
        else
        {
            request.RequestUri = new Uri("https://api.spotify.com/v1/me/playlists?limit=50");
        }
        request.Method = HttpMethod.Get;

        request.Headers.Add("Authorization", "Bearer " + accessToken);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var response = await client.SendAsync(request);
        var result = await response.Content.ReadAsStringAsync();
        var playlists = JsonSerializer.Deserialize<Playlists>(result);
        return playlists;
    }
    public static async Task<List<string>> GetAllUsersPlaylists(string accessToken)
    {
        List<string> allPlaylists = new List<string>();
        var currentPage = GetPageOfUsersPlaylists(accessToken);
        allPlaylists.AddRange(currentPage.Result.items.Select(item => item.name));
        Console.WriteLine("Total playlists: " + allPlaylists.Count);
        while (currentPage.Result.next != null)
        {
            currentPage = GetPageOfUsersPlaylists(accessToken, currentPage.Result.next);
            allPlaylists.AddRange(currentPage.Result.items.Select(item => item.name));
            Console.WriteLine("Total playlists: " + allPlaylists.Count);
        }
        return allPlaylists;
    }
    public static async Task<(Dictionary<string, List<string>>, HashSet<string>)> GetSongGenresDict(string accessToken, HashSet<Models.LikedSongs.Item> likedSongs)
    {
        HashSet<string> allArtistIDs = new HashSet<string>();
        Dictionary<string, List<string>> songs_artistIDs = new Dictionary<string, List<string>>();

        // For each song in the liked songs list, create a dictionary with the song name as the key and a list of artist IDs as the value
        foreach (var song in likedSongs)
        {
            var artistIDs = song.track.artists.Select(a => a.id).ToList();
            songs_artistIDs[song.track.name] = artistIDs;
            foreach (var id in artistIDs)
                allArtistIDs.Add(id);
        }

        // Batch artist IDs (max 50 per request)
        var artistIDBatches = allArtistIDs
            .Select((id, idx) => new { id, idx })
            .GroupBy(x => x.idx / 50)
            .Select(g => g.Select(x => x.id).ToList())
            .ToList();

        // For each song, send an API request to the Get Several Artists endpoint with the ids of artists from the liked songs list
        // and get the genres of each artist
        // Store the genres in a dictionary with the song name as the key and a list of genres as the value
        var client = new HttpClient();
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        Dictionary<string, List<string>> artistGenres = new Dictionary<string, List<string>>();
        HashSet<string> uniqueGenres = new HashSet<string>();
        foreach (var batch in artistIDBatches)
        {
            bool success = false;
            while (!success)
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"https://api.spotify.com/v1/artists?ids={string.Join(",", batch)}");
                request.Headers.Add("Authorization", "Bearer " + accessToken);
                var response = await client.SendAsync(request);

                if ((int)response.StatusCode == 429)
                {
                    // Rate limited, wait and retry
                    if (response.Headers.TryGetValues("Retry-After", out var values))
                    {
                        int waitSeconds = int.Parse(values.First());
                        Console.WriteLine($"Rate limited. Waiting {waitSeconds} seconds...");
                        await Task.Delay(waitSeconds * 1000);
                    }
                    else
                    {
                        await Task.Delay(5000); // fallback wait
                    }
                    continue;
                }
                var result = await response.Content.ReadAsStringAsync();
                var deserializedArtists = JsonSerializer.Deserialize<ArtistList>(result);
                if (deserializedArtists?.artists != null)
                {
                    foreach (var artist in deserializedArtists.artists)
                    {
                        artistGenres[artist.id] = artist.genres ?? new List<string>();
                        uniqueGenres.UnionWith(artist.genres ?? Enumerable.Empty<string>());
                    }
                }
                success = true;
            } 
        }
        
            // Now, for each song, replace artist IDs with genres
            var songs_genres = new Dictionary<string, List<string>>();
            foreach (var kvp in songs_artistIDs)
            {
                var genres = kvp.Value
                    .Where(id => artistGenres.ContainsKey(id))
                    .SelectMany(id => artistGenres[id])
                    .Distinct()
                    .ToList();
                songs_genres[kvp.Key] = genres;
            }
            return (songs_genres, uniqueGenres);
    }
    public static async Task<CreatedPlaylist> CreatePlaylist(string accessToken, string userID, string genre)
    {
        //make a post request to https://api.spotify.com/v1/users/{user_id}/playlists
        var client = new HttpClient();
        var request = new HttpRequestMessage();
        request.RequestUri = new Uri($"https://api.spotify.com/v1/users/{userID}/playlists");
        request.Method = HttpMethod.Post;
        request.Headers.Add("Authorization", "Bearer " + accessToken);
        client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        var requestData = new
        {
            name = genre,
            description = "Playlist created by SpotifyAPIService",
            @public = true
        };
        string json = JsonSerializer.Serialize(requestData);
        var content = new StringContent(json, Encoding.UTF8, "application/json");
        var response = await client.PostAsync(request.RequestUri, content);
        var result = await response.Content.ReadAsStringAsync();
        var playlist = JsonSerializer.Deserialize<CreatedPlaylist>(result);
        return playlist;
        //using the input genre name as the name of the playlist
        //and the description as "Playlist created by SpotifyAPIService"
        //put all songs in the songs_artistIDs dictionary into the playlist that have the input genre in the values list
    }
    public static async Task<string> AddSongsToPlaylist(string accessToken, string playlist_id, List<string> songUris)
    {
        //split the songsUrisToAdd list into batches of 100
        var songUriBatches = songUris
            .Select((uri, idx) => new { uri, idx })
            .GroupBy(x => x.idx / 100)
            .Select(g => g.Select(x => x.uri).ToList())
            .ToList();
        int totalSongsAdded = 0;
        foreach (var batch in songUriBatches)
        {
            if (batch.Count == 0)
                continue;

            //make a post request to https://api.spotify.com/v1/playlists/{playlist_id}/tracks
            var client = new HttpClient();
            var request = new HttpRequestMessage();
            request.RequestUri = new Uri($"https://api.spotify.com/v1/playlists/{playlist_id}/tracks");
            request.Method = HttpMethod.Post;
            request.Headers.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var requestData = new
            {
                uris = batch
            };
            string json = JsonSerializer.Serialize(requestData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync(request.RequestUri, content);
            var result = await response.Content.ReadAsStringAsync();
            var addedSongToPlaylist = JsonSerializer.Deserialize<AddedSongToPlaylist>(result);
            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to add songs to playlist. Response: " + result);
            }
            else
            {
                totalSongsAdded += batch.Count;
            }
        }
        if (songUriBatches.Count > 0)
        {
            return $"Added {totalSongsAdded} songs to the playlist.";
        }
        else
        {
            return "No songs added to the playlist.";
        }
    }
}