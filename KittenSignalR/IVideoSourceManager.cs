using KittenSignalR.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KittenSignalR
{
    public interface IVideoSourceManager
    {
        List<Video> GetVideos();

        Task<List<Video>> GetVideosFromCreator(Creator creator, int maxVideos);

        List<Video> SetNewVideoFromCreator(Video video, Creator creator);

        void DeleteVideo(Video video);

        void DeleteVideo(Video video, Creator creator);
    }
}