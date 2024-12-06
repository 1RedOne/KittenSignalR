using System.Collections.Generic;

namespace KittenSignalR.Models
{
    public class CreatorVideos
    {
        public string CreatorName { get; set; }
        public List<Video> Videos { get; set; }
    }
}