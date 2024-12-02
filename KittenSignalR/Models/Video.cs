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
        public string VideoThumbnail { get; set; }
    }
}