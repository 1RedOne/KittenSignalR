using System;
using System.Collections.Generic;
using System.Linq;

namespace KittenSignalR.Models
{
    public class ShoppingCart
    {
        public int OrderNumberId { get; set; }
        public string UserName { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsActiveOrder { get; set; }
        public bool OrderApproved { get; set; }
        public bool OrderFulfilled { get; set; }
        public List<Video> OrderItems { get; set; }
        public bool PendingApproval { get; internal set; }

        public ShoppingCart()
        {
            this.OrderItems = new List<Video>();
        }

        public void SafeAddToCart(Video videoToAdd)
        {
            if (null == this.OrderItems)
            {
                this.OrderItems = new List<Video>(0);
            }

            var matchingVideo = this.OrderItems.Any(x => x.VideoId == videoToAdd.VideoId);

            if (matchingVideo)
            {
                return;
            }

            this.OrderItems.Add(videoToAdd);
        }

        public void SafeDelete(Video desiredVideo)
        {
            if (null == this.OrderItems)
            {
                this.OrderItems = new List<Video>(0);
            }

            var matchingVideo = this.OrderItems.Any(x => x.VideoId == desiredVideo.VideoId);

            if (matchingVideo)
            {
                this.OrderItems.Remove(this.OrderItems.FirstOrDefault(x => x.VideoId == desiredVideo.VideoId));
            }
            
        }

        public void SafeDeleteByVideoUri(string videoUri)
        {
            if (null == this.OrderItems)
            {
                this.OrderItems = new List<Video>(0);
            }

            var matchingVideo = this.OrderItems.Any(x => x.VideoUrl == videoUri);

            if (matchingVideo)
            {
                this.OrderItems.Remove(this.OrderItems.FirstOrDefault(x => x.VideoUrl == videoUri));
            }
        }
    }
}