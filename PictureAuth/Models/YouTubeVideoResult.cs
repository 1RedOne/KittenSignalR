using System;

namespace KittenSignalR.Models
{
    public class YouTubeVideoResult
    {
        public string VideoId { get; set; }
        public string Description { get; set; }
        public string YtThumbnail { get; set; }
        public string VideoTitle { get; set; }
        public DateTime PublishedAt { get; set; }
        public string cachedThumbnail { get; set; }

        public YouTubeVideoResult(Item item)
        {
            this.PublishedAt = item.contentDetails.videoPublishedAt;
            this.Description = item.snippet.description;
            this.VideoTitle = item.snippet.title;
            this.YtThumbnail = item.snippet.thumbnails.@default.url;
            this.cachedThumbnail = null;
            this.VideoId = item.contentDetails.videoId;
        }

        public YouTubeVideoResult()
        { }
    }
}