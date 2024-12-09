using ABI.Windows.ApplicationModel.Activation;
using KittenSignalR.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.Extensions.Logging;
using PictureAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KittenSignalR.Repositories
{
    public class ShoppingCartManager : IShoppingCartManager

    {
        private List<ShoppingCart> shoppingCartList;

        private ILogger<ShoppingCartManager> logger;

        private string cartDirExpectedPath = $"shoppingCarts";
        private string cartExpectedPath = $"shoppingCarts/shoppingCartList.json";

        public ShoppingCartManager(IYouTubeRepo youTubeRepo, ILogger<ShoppingCartManager> logger)
        {
            this.logger = logger;
            this.shoppingCartList = WarmUpShoppingCartList();
        }

        private List<ShoppingCart>? WarmUpShoppingCartList()
        {
            this.logger?.BeginScope("[JsonShoppingCartStoreSetup]");
            this.EnsureShoppingCartListReady();

            this.logger?.LogInformation("Loading shopping cart list");
            var jsonContent = System.IO.File.ReadAllText(this.cartExpectedPath);

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return new List<ShoppingCart>();
            }

            // Deserialize the JSON into a List<Channel>
            var warmingShoppingCartList = JsonSerializer.Deserialize<List<ShoppingCart>>(jsonContent);
            this.logger?.LogInformation($"Online with ${warmingShoppingCartList.Count} carts");

            return warmingShoppingCartList;
        }

        private void EnsureShoppingCartListReady()
        {
            this.logger?.LogInformation("Ensuring user list path is ready");
            if (!System.IO.File.Exists(cartExpectedPath))
            {
                if (!System.IO.Directory.Exists(cartDirExpectedPath))
                {
                    this.logger?.LogInformation("Need to create source directory");

                    System.IO.Directory.CreateDirectory(cartDirExpectedPath);

                    this.logger?.LogInformation("Created source directory");
                }

                var list = new List<ShoppingCart>();

                WriteShoppingCarts(cartExpectedPath, list);
            }

            this.logger?.LogInformation("User list path is ready");
        }

        private void WriteShoppingCarts(string userExpectedPath, List<ShoppingCart> list)
        {
            var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(list, serializerOptions);

            System.IO.File.WriteAllText(userExpectedPath, jsonContent);
        }

        private void RefreshCartList()
        {
            this.WriteShoppingCarts(this.cartExpectedPath, this.shoppingCartList);
            this.logger?.LogInformation($"[RefreshCartList], list now has {this.shoppingCartList.Count()} members");
        }

        public ShoppingCart GetCartByUser(RegisterDto user)
        {
            //look through active lists to see if we have a cart for this user
            var cart = this.GetExistingCartOrNewCartForUser(user);
            this.RefreshCartList();
            return cart;
        }

        public List<ShoppingCart> GetAllCartsByUserId(int userId)
        {
            var carts = shoppingCartList.Where(x => x.UserId == userId);

            return carts.ToList();
        }

        public ShoppingCart AddToCartByUser(RegisterDto user, Video video)
        {
            //look through active lists to see if we have a cart for this user
            var cart = this.GetExistingCartOrNewCartForUser(user);

            cart.OrderItems.Add(video);

            this.RefreshCartList();

            return cart;
        }

        private ShoppingCart GetExistingCartOrNewCartForUser(RegisterDto user)
        {
            var cart = shoppingCartList.Where(x => x.IsActiveOrder)?.FirstOrDefault(x => x.UserId == user.Id);

            if (null == cart)
            {
                cart = this.NewCartForUser(user);
                this.shoppingCartList.Add(cart);
            }

            return cart;
        }

        private ShoppingCart NewCartForUser(RegisterDto user)
        {
            return new ShoppingCart
            {
                OrderNumberId = shoppingCartList.Count + 1,
                UserId = user.Id,
                UserName = user.Name,
                OrderDate = DateTime.Now,
                IsActiveOrder = true,
                OrderApproved = false,
                OrderFulfilled = false
            };
        }

        private ShoppingCart NewCartForUser(int userId, Video video)
        {
            return new ShoppingCart
            {
                OrderNumberId = shoppingCartList.Count + 1,
                UserId = userId,
                UserName = "test",
                OrderDate = DateTime.Now,
                IsActiveOrder = true,
                OrderApproved = false,
                OrderFulfilled = false,
                OrderItems = new List<Video> { video }
            };
        }

        public ShoppingCart AddToCart(int orderNumber, Video video)
        {
            var cart = shoppingCartList.FirstOrDefault(x => x.OrderNumberId == orderNumber);

            if (null == cart)
            {
                return null;
            }

            cart.OrderItems.Add(video);

            WriteShoppingCarts(cartExpectedPath, shoppingCartList);

            return cart;
        }

        public ShoppingCart RemoveFromCart(int orderNumber, Video video)
        {
            var cart = shoppingCartList.FirstOrDefault(x => x.OrderNumberId == orderNumber);

            if (null == cart)
            {
                return null;
            }

            cart.OrderItems.Remove(video);

            WriteShoppingCarts(cartExpectedPath, shoppingCartList);

            return cart;
        }

        public ShoppingCart Checkout(int orderNumber)
        {
            var cart = shoppingCartList.FirstOrDefault(x => x.OrderNumberId == orderNumber);

            if (null == cart)
            {
                return null;
            }

            cart.IsActiveOrder = true;
            cart.PendingApproval = true;
            cart.OrderApproved = false;

            WriteShoppingCarts(cartExpectedPath, shoppingCartList);

            return cart;
        }

        public ShoppingCart EmptyCart(int orderNumber)
        {
            var cart = shoppingCartList.FirstOrDefault(x => x.OrderNumberId == orderNumber);

            if (null == cart)
            {
                return null;
            }

            cart.OrderItems.Clear();

            WriteShoppingCarts(cartExpectedPath, shoppingCartList);

            return cart;
        }

        public List<ShoppingCart> ListAllOrders()
        {
            return this.shoppingCartList;
        }

        public ShoppingCart OrderDetails(int orderNumber)
        {
            var cart = this.shoppingCartList.FirstOrDefault(x => x.OrderNumberId == orderNumber);

            return cart;
        }

        public void RefreshCart(ShoppingCart thisUsersCart)
        {
            var matchingCart = this.shoppingCartList.FirstOrDefault(x => x.OrderNumberId == thisUsersCart.OrderNumberId);

            matchingCart = thisUsersCart;

            RefreshCartList();
        }

        public int GetItemsCountByUser(RegisterDto userRecord)
        {
            var cart = this.GetCartByUser(userRecord);

            return cart.OrderItems.Count;
        }
    }
}