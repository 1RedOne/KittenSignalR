using System.Threading.Tasks;
using KittenSignalR.Models;

namespace KittenSignalR
{
    public interface IYouTubeRepo
    {
        Task<YouTubeChannelResult> InvokeYoutubeCreatorSearchAsync(string query);
    }
}