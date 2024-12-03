using System.Collections.Generic;
using System;

namespace KittenSignalR.Models
{
    public class YouTubeChannelResult
    {
        private Item item;

        public string Title { get; set; }
        public string Description { get; set; }
        public string CustomUrl { get; set; }
        public DateTime PublishedAt { get; set; }
        public string ThumbnailUrl { get; set; }
        public RelatedPlaylists linkedPlaylists { get; set; }

        public YouTubeChannelResult(Snippet snippet, ContentDetails details)
        {
        }

        public YouTubeChannelResult(Item item)
        {
            this.item = item;
            this.SetProperties();
        }

        private void SetProperties()
        {
            var snippet = item.snippet;
            var details = item.contentDetails;

            this.PublishedAt = snippet.publishedAt;
            this.Title = snippet.title;
            this.Description = snippet.description;
            this.CustomUrl = snippet.customUrl;
            this.ThumbnailUrl = snippet.thumbnails.@default.url;
            this.linkedPlaylists = details.relatedPlaylists;
        }
    }
}