using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KittenSignalR.Models
{
    public class Video
    {
        public string CreatorId { get; set; }
        public string VideoUrl { get; set; }
        public string VideoTitle { get; set; }
        public string VideoDescription { get; set; }
        public int VideoLength { get; set; }
    }

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
            this.ChannelId = channelResult.CustomUrl;
            this.ChannelName = channelResult.Title;
            this.ChannelUrl = channelResult.CustomUrl;
            this.ChannelDescription = channelResult.Description;
            this.ThumbnailUri = channelResult.ThumbnailUrl;
        }
    }
}