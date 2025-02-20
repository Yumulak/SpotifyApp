// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
namespace Models.SeveralArtists{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Artist
    {
        [JsonPropertyName("external_urls")]
        public ExternalUrls external_urls { get; set; }

        [JsonPropertyName("followers")]
        public Followers followers { get; set; }

        [JsonPropertyName("genres")]
        public List<string> genres { get; set; }

        [JsonPropertyName("href")]
        public string href { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("images")]
        public List<Image> images { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("popularity")]
        public int popularity { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("uri")]
        public string uri { get; set; }
    }

    public class ExternalUrls
    {
        [JsonPropertyName("spotify")]
        public string spotify { get; set; }
    }

    public class Followers
    {
        [JsonPropertyName("href")]
        public string href { get; set; }

        [JsonPropertyName("total")]
        public int total { get; set; }
    }

    public class Image
    {
        [JsonPropertyName("url")]
        public string url { get; set; }

        [JsonPropertyName("height")]
        public int height { get; set; }

        [JsonPropertyName("width")]
        public int width { get; set; }
    }

    public class ArtistList
    {
        [JsonPropertyName("artists")]
        public List<Artist> artists { get; set; }
    }


}
    