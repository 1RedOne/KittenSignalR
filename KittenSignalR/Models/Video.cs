using NuGet.Protocol.Plugins;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace KittenSignalR.Models
{
    public class Video : IEquatable<Video>
    {
        public string CreatorId { get; set; }

        public string CreatorChannelName { get; private set; }
        public string VideoUrl { get; set; }

        public string VideoId { get; set; }

        public string VideoTitle { get; set; }

        public string VideoDescription { get; set; }

        public int VideoLength { get; set; }

        public string VideoThumbnail { get; set; }

        public DateTime CreatedDate { get; set; }

        public bool IsLive { get; set; }
        public string PlexUrl { get; set; }

        public Video(YouTubeVideoResult videoResult, Creator creator)
        {
            this.CreatorId = creator.ChannelId;
            this.CreatorChannelName = creator.ChannelName;
            this.VideoId = videoResult.VideoId;
            this.VideoTitle = videoResult.VideoTitle;
            this.VideoDescription = videoResult.Description;
            this.VideoThumbnail = videoResult.YtThumbnail;
            this.VideoLength = 0;
            this.CreatedDate = videoResult.PublishedAt;
        }

        public Video()
        {
            this.CreatorChannelName = this.CreatorChannelName ?? this.CreatorId;
        }

        public void SetupCreatorChannelName()
        {
            this.CreatorChannelName = this.CreatorChannelName ?? this.CreatorId;
        }

        public void TrimExcessiveDescriptions()
        {
            var descriptionLength = this.VideoDescription.Length;
            if (descriptionLength > 255)
            {
                descriptionLength = 255;
            }

            if (descriptionLength == 0)
            {
                return;
            }

            this.VideoDescription.Substring(0, descriptionLength - 1);
        }

        public string GetDateAgo()
        {
            var now = DateTime.Now;

            var diff = now - this.CreatedDate;

            //if less than a day, return hours ago, otherwise return days ago
            if (diff.TotalHours < 24)
            {
                return $"{Math.Abs(Math.Round(diff.TotalHours, 0))} hours ago";
            }
            else
            {
                return $"{Math.Abs(Math.Round(diff.TotalDays, 0))} days ago";
            }
        }

        public bool Equals(Video other)
        {
            if (null == other)
            {
                return false;
            }

            return this.VideoId == other.VideoId &&
                this.CreatorId == other.CreatorId;
        }

        public override bool Equals(object other)
         => other is Video otherVid && this.Equals(otherVid);

        public override int GetHashCode()
        {
            var videoIdHash = VideoId == null ? 0 : VideoId.GetHashCode();
            var creatorHash = CreatorId == null ? 0 : CreatorId.GetHashCode();

            return videoIdHash + creatorHash;
        }
    }
}