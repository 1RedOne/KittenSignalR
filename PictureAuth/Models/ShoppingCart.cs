using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KittenSignalR.Models
{
    public class ShoppingCart
    {
        [Key]
        public int OrderNumberId { get; set; }

        public string UserName { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; }
        public bool IsActiveOrder { get; set; }
        public bool OrderApproved { get; set; }
        public bool OrderFulfilled { get; set; }
        public List<Video> OrderItems { get; set; }
    }
}