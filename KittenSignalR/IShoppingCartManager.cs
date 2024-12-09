using KittenSignalR.Models;
using PictureAuth.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KittenSignalR.Repositories
{
    public interface IShoppingCartManager
    {
        ShoppingCart GetCartByUser(RegisterDto user);

        List<ShoppingCart> GetAllCartsByUserId(int userId);

        List<ShoppingCart> ListAllOrders();

        ShoppingCart AddToCartByUser(RegisterDto user, Video video);

        ShoppingCart AddToCart(int orderNumber, Video video);

        ShoppingCart RemoveFromCart(int orderNumber, Video video);

        ShoppingCart Checkout(int orderNumber);

        ShoppingCart EmptyCart(int orderNumber);

        ShoppingCart OrderDetails(int orderNumber);

        void RefreshCart(ShoppingCart thisUsersCart);

        int GetItemsCountByUser(RegisterDto userRecord);
    }
}