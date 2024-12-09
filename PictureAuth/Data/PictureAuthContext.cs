using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using KittenSignalR.Models;

namespace PictureAuth.Data
{
    public class PictureAuthContext : DbContext
    {
        public PictureAuthContext (DbContextOptions<PictureAuthContext> options)
            : base(options)
        {
        }

        public DbSet<KittenSignalR.Models.ShoppingCart> ShoppingCart { get; set; }
    }
}
