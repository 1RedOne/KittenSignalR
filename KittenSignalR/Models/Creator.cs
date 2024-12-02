using System.Text.Json.Serialization;

namespace KittenSignalR.Models
{
    public class Creator
    {
        [JsonPropertyName("channelId")]
        public string ChannelId { get; set; }

        [JsonPropertyName("channelName")]
        public string ChannelName { get; set; }

        [JsonPropertyName("channelUrl")]
        public string ChannelUrl { get; set; }

        [JsonPropertyName("channelDescription")]
        public string ChannelDescription { get; set; }

        [JsonPropertyName("thumbnailUri")]
        public string ThumbnailUri { get; set; }

        public Creator()
        { }

        public Creator(YouTubeChannelResult channelResult)
        {
            ChannelId = channelResult.CustomUrl;
            ChannelName = channelResult.Title;
            ChannelUrl = channelResult.CustomUrl;
            ChannelDescription = channelResult.Description;
            ThumbnailUri = channelResult.ThumbnailUrl;
        }
    }
}