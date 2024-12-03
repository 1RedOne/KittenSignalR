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
        
        public string VideoId { get; set; }

        public string VideoTitle { get; set; }

        public string VideoDescription { get; set; }

        public int VideoLength { get; set; }
        
        public string VideoThumbnail { get; set; }

        public DateTime CreatedDate { get; set; }

        public Video(YouTubeVideoResult videoResult, Creator creator)
        {
            this.CreatorId = creator.ChannelId;
            this.VideoId = videoResult.VideoId;
            this.VideoTitle = videoResult.VideoTitle;
            this.VideoDescription = videoResult.Description;
            this.VideoThumbnail = videoResult.YtThumbnail;
            this.VideoLength = 0;
            this.CreatedDate = videoResult.PublishedAt;
        }

        public Video()
        { }
    }
}