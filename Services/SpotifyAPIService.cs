using Models.SeveralArtists;

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
        if(accessToken != null){
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
        if(url != null){
            request.RequestUri = new Uri(url);
        }
        else{
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
    public static async Task<(List<string>, HashSet<Models.LikedSongs.Item>)> GetAllUsersLikedSongs(string accessToken){
        List<string> allSongs = new List<string>();
        HashSet<Models.LikedSongs.Item> allLikedSongObject = new HashSet<Models.LikedSongs.Item>();
        var currentPage = GetPageOfUsersLikedSongs(accessToken);
        allLikedSongObject.UnionWith(currentPage.Result.items);
        allSongs.AddRange(currentPage.Result.items.Select(item => item.track.name));
        Console.WriteLine("Total songs: " + allSongs.Count);
        while(currentPage.Result.next != null){
            currentPage = GetPageOfUsersLikedSongs(accessToken, currentPage.Result.next);
            allSongs.AddRange(currentPage.Result.items.Select(item => item.track.name));
            allLikedSongObject.UnionWith(currentPage.Result.items);
            Console.WriteLine("Total songs: " + allSongs.Count);
        }
        return (allSongs, allLikedSongObject);
    }
    private static async Task<Playlists> GetPageOfUsersPlaylists(string accessToken, string? url = null)
    {
        var client = new HttpClient();
        var request = new HttpRequestMessage();
        if(url != null){
            request.RequestUri = new Uri(url);
        }
        else{
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
    public static async Task<List<string>> GetAllUsersPlaylists(string accessToken){
        List<string> allPlaylists = new List<string>();
        var currentPage = GetPageOfUsersPlaylists(accessToken);
        allPlaylists.AddRange(currentPage.Result.items.Select(item => item.name));
        Console.WriteLine("Total playlists: " + allPlaylists.Count);
        while(currentPage.Result.next != null){
            currentPage = GetPageOfUsersPlaylists(accessToken, currentPage.Result.next);
            allPlaylists.AddRange(currentPage.Result.items.Select(item => item.name));
            Console.WriteLine("Total playlists: " + allPlaylists.Count);
        }
        return allPlaylists;
    }
    public static async Task<Dictionary<string, List<string>>> GetSongGenresDict(string accessToken, HashSet<Models.LikedSongs.Item> likedSongs){
        List<Models.SeveralArtists.Artist> allArtists = new List<Models.SeveralArtists.Artist>();
        List<string> artistIDs = new List<string>();
        Dictionary<string, List<string>> songs_artists = new Dictionary<string, List<string>>();
        var client = new HttpClient();
        var request = new HttpRequestMessage();

        foreach(var song in likedSongs){
            artistIDs.AddRange(song.track.artists.Select(a => a.id));
            songs_artists.Add(song.track.name, artistIDs);
            artistIDs.Clear();
        }

        foreach(var songArtistListPair in songs_artists){        
            request.RequestUri = new Uri($"https://api.spotify.com/v1/artists?ids={string.Join(",", songArtistListPair.Value)}");
            request.Method = HttpMethod.Get;
            request.Headers.Add("Authorization", "Bearer " + accessToken);
            client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await client.SendAsync(request);
            var result = await response.Content.ReadAsStringAsync();
            var deserializedArtists = JsonSerializer.Deserialize<ArtistList>(result);
            allArtists.AddRange(deserializedArtists.artists);
            foreach( var artist in deserializedArtists.artists){
                foreach(var kvp in songs_artists){
                    kvp.Value.Clear();
                    kvp.Value.AddRange(artist.genres);
                }
            }
        }
        return songs_artists;
    }    
}   