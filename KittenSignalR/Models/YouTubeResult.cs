using System.Collections.Generic;
using System;

namespace KittenSignalR.Models
{
    // Root myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class ContentDetails
    {
        public RelatedPlaylists relatedPlaylists { get; set; }
        public DateTime videoPublishedAt { get; set; }
        public string videoId { get; set; }
    }

    public class Default
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class High
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class Item
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public string id { get; set; }
        public Snippet snippet { get; set; }
        public ContentDetails contentDetails { get; set; }
        public Statistics statistics { get; set; }
    }

    public class Localized
    {
        public string title { get; set; }
        public string description { get; set; }
    }

    public class Medium
    {
        public string url { get; set; }
        public int width { get; set; }
        public int height { get; set; }
    }

    public class PageInfo
    {
        public int totalResults { get; set; }
        public int resultsPerPage { get; set; }
    }

    public class RelatedPlaylists
    {
        public string likes { get; set; }
        public string uploads { get; set; }
    }

    public class YouTubeSearchResponse
    {
        public string kind { get; set; }
        public string etag { get; set; }
        public PageInfo pageInfo { get; set; }
        public List<Item> items { get; set; }
    }

    public class Snippet
    {
        public string title { get; set; }
        public string description { get; set; }
        public string customUrl { get; set; }
        public DateTime publishedAt { get; set; }
        public Thumbnails thumbnails { get; set; }
        public Localized localized { get; set; }
        public string country { get; set; }
    }

    public class Statistics
    {
        public string viewCount { get; set; }
        public string subscriberCount { get; set; }
        public bool hiddenSubscriberCount { get; set; }
        public string videoCount { get; set; }
    }

    public class Thumbnails
    {
        public Default @default { get; set; }
        public Medium medium { get; set; }
        public High high { get; set; }
    }
}