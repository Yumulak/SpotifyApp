// Playlists listOfPlaylists = JsonSerializer.Deserialize<Playlists>(myJsonResponse);
    namespace Models.Playlists{
        // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
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
        public int? height { get; set; }

        [JsonPropertyName("width")]
        public int? width { get; set; }
    }

    public class Item
    {
        [JsonPropertyName("collaborative")]
        public bool collaborative { get; set; }

        [JsonPropertyName("description")]
        public string description { get; set; }

        [JsonPropertyName("external_urls")]
        public ExternalUrls external_urls { get; set; }

        [JsonPropertyName("href")]
        public string href { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("images")]
        public List<Image> images { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("owner")]
        public Owner owner { get; set; }

        [JsonPropertyName("public")]
        public bool @public { get; set; }

        [JsonPropertyName("snapshot_id")]
        public string snapshot_id { get; set; }

        [JsonPropertyName("tracks")]
        public Tracks tracks { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("uri")]
        public string uri { get; set; }
    }

    public class Owner
    {
        [JsonPropertyName("external_urls")]
        public ExternalUrls external_urls { get; set; }

        [JsonPropertyName("followers")]
        public Followers followers { get; set; }

        [JsonPropertyName("href")]
        public string href { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("uri")]
        public string uri { get; set; }

        [JsonPropertyName("display_name")]
        public string display_name { get; set; }
    }

    public class Playlists
    {
        [JsonPropertyName("href")]
        public string href { get; set; }

        [JsonPropertyName("limit")]
        public int limit { get; set; }

        [JsonPropertyName("next")]
        public string next { get; set; }

        [JsonPropertyName("offset")]
        public int offset { get; set; }

        [JsonPropertyName("previous")]
        public string previous { get; set; }

        [JsonPropertyName("total")]
        public int total { get; set; }

        [JsonPropertyName("items")]
        public List<Item> items { get; set; }
    }

    public class Tracks
    {
        [JsonPropertyName("href")]
        public string href { get; set; }

        [JsonPropertyName("total")]
        public int total { get; set; }
    }


}
    
