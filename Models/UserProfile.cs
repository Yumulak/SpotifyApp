// UserProfile myDeserializedClass = JsonConvert.DeserializeObject<UserProfile>(myJsonResponse);
    public class ExplicitContent
    {
        [JsonPropertyName("filter_enabled")]
        public bool filter_enabled { get; set; }

        [JsonPropertyName("filter_locked")]
        public bool filter_locked { get; set; }
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

    public class UserProfile
    {
        [JsonPropertyName("country")]
        public string country { get; set; }

        [JsonPropertyName("display_name")]
        public string display_name { get; set; }

        [JsonPropertyName("email")]
        public string email { get; set; }

        [JsonPropertyName("explicit_content")]
        public ExplicitContent explicit_content { get; set; }

        [JsonPropertyName("external_urls")]
        public ExternalUrls external_urls { get; set; }

        [JsonPropertyName("followers")]
        public Followers followers { get; set; }

        [JsonPropertyName("href")]
        public string href { get; set; }

        [JsonPropertyName("id")]
        public string id { get; set; }

        [JsonPropertyName("images")]
        public List<Image> images { get; set; }

        [JsonPropertyName("product")]
        public string product { get; set; }

        [JsonPropertyName("type")]
        public string type { get; set; }

        [JsonPropertyName("uri")]
        public string uri { get; set; }
    }

