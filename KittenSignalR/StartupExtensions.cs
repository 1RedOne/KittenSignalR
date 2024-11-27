using KittenSignalR.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using System;
using System.IO;

namespace KittenSignalR
{
    public static class StartupExtensions
    {
        public static void ConfigureDownloadDirectory(this IApplicationBuilder app)
        {
            var targetPath = Path.Combine(Directory.GetCurrentDirectory(), "youtubeDLs");
            //create if missing
            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }
        }

        public static void ConfigureYoutubeOptions(this IServiceCollection services)
        {
            services.Configure<YouTubeRepoOptions>(options =>
            {
                //YouTubeRepoOptions
                var apikey =
                 Environment.GetEnvironmentVariable("APIKey");

                if (null == apikey)
                {
                    var credentialSourceUrl = "https://console.cloud.google.com/apis/credentials?inv=1&invt=AbinVQ&project=algebraic-cycle-279319";
                    throw new Exception($"go to {credentialSourceUrl} to set value for APIkey");
                }
                options.ApiKey = apikey;
            });
        }
    }
}