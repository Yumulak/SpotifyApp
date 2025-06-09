// Root myDeserializedClass = JsonSerializer.Deserialize<Root>(myJsonResponse);
    public class AddedSongToPlaylist
    {
        [JsonPropertyName("snapshot_id")]
        public string SnapshotId { get; set; }
    }