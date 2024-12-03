using System.Collections.Generic;
using System.Threading.Tasks;
using KittenSignalR.Models;

namespace KittenSignalR
{
    public interface IYouTubeRepo
    {
        Task<YouTubeChannelResult> InvokeYoutubeCreatorSearchAsync(string query);

        Task<YouTubeVideoResult> InvokeYoutubeVideoSearchAsync(string creator, int maxCount);

        Task<List<YouTubeVideoResult>> InvokeYoutubeVideoSearchAsync2(Creator creator, int maxCount);
    }
}