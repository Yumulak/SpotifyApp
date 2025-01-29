// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
namespace Models.LikedSongs{
    // Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class Album
    {
        [JsonPropertyName("album_type")]
        public string album_type { get; set; }

        [JsonPropertyName("total_tracks")]
        public int total_tracks { get; set; }

        [JsonPropertyName("available_markets")]
        public List<string> available_markets { get; set; }

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

        [JsonPropertyName("release_date")]
        public string release_date { get; set; }

        [JsonPropertyName("release_date_precision")]
        public string release_date_precision { get; set; }

        [JsonPropertyName("restrictions")]
        public Restrictions restrictions { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("uri")]
        public string uri { get; set; }

        [JsonPropertyName("artists")]
        public List<Artist> artists { get; set; }
    }

    public class Artist
    {
        [JsonPropertyName("external_urls")]
        public ExternalUrls external_urls { get; set; }

        [JsonPropertyName("href")]
        public string href { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("uri")]
        public string uri { get; set; }
    }

    public class ExternalIds
    {
        [JsonPropertyName("isrc")]
        public string isrc { get; set; }

        [JsonPropertyName("ean")]
        public string ean { get; set; }

        [JsonPropertyName("upc")]
        public string upc { get; set; }
    }

    public class ExternalUrls
    {
        [JsonPropertyName("spotify")]
        public string spotify { get; set; }
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

    public class Item
    {
        [JsonPropertyName("added_at")]
        public string added_at { get; set; }

        [JsonPropertyName("track")]
        public Track track { get; set; }
    }

    public class LinkedFrom
    {
    }

    public class Restrictions
    {
        [JsonPropertyName("reason")]
        public string reason { get; set; }
    }

    public class LikedSongs
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

    public class Track
    {
        [JsonPropertyName("album")]
        public Album album { get; set; }

        [JsonPropertyName("artists")]
        public List<Artist> artists { get; set; }

        [JsonPropertyName("available_markets")]
        public List<string> available_markets { get; set; }

        [JsonPropertyName("disc_number")]
        public int disc_number { get; set; }

        [JsonPropertyName("duration_ms")]
        public int duration_ms { get; set; }

        [JsonPropertyName("explicit")]
        public bool @explicit { get; set; }

        [JsonPropertyName("external_ids")]
        public ExternalIds external_ids { get; set; }

        [JsonPropertyName("external_urls")]
        public ExternalUrls external_urls { get; set; }

        [JsonPropertyName("href")]
        public string href { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("is_playable")]
        public bool is_playable { get; set; }

        [JsonPropertyName("linked_from")]
        public LinkedFrom linked_from { get; set; }

        [JsonPropertyName("restrictions")]
        public Restrictions restrictions { get; set; }

        [JsonPropertyName("name")]
        public string name { get; set; }

        [JsonPropertyName("popularity")]
        public int popularity { get; set; }

        [JsonPropertyName("preview_url")]
        public string preview_url { get; set; }

        [JsonPropertyName("track_number")]
        public int track_number { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("uri")]
        public string uri { get; set; }

        [JsonPropertyName("is_local")]
        public bool is_local { get; set; }
    }


}