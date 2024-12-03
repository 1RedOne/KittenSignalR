using KittenSignalR.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace KittenSignalR.Repositories
{
    public class VideoSourceManager : IVideoSourceManager
    {
        private List<Creator> creatorList;
        private IYouTubeRepo youTubeRepo;

        public VideoSourceManager(IYouTubeRepo youTubeRepo)
        {
            this.creatorList = GetCreators();
            this.youTubeRepo = youTubeRepo;
        }

        public async Task<List<Video>> GetVideos(Creator creator)
        {
            //add console logigng that we are loading the list
            Console.WriteLine($"Loading video list for {creator.ChannelName}");

            //check if creator file exists
            var creatorExpectedPath = $"videos/{creator.ChannelName}.json";

            if (!System.IO.File.Exists(creatorExpectedPath))
            {
                Console.WriteLine($"No video list found for {creator.ChannelName}");

                //call new internal method to retrieve first five videos from creator
                var videos = await this.GetVideosFromCreator(creator, 5);
                return new List<Video>();
            }

            var jsonContent = System.IO.File.ReadAllText("videoList.json");

            // Deserialize the JSON into a List<Channel>
            var videosDefault = JsonSerializer.Deserialize<List<Video>>(jsonContent);

            return videosDefault;
        }

        public List<Creator> GetCreators()
        {
            //add console logigng that we are loading the list
            Console.WriteLine("Loading creator list");
            var jsonContent = System.IO.File.ReadAllText("creatorList.json");

            // Deserialize the JSON into a List<Channel>
            var creators = JsonSerializer.Deserialize<List<Creator>>(jsonContent);

            return creators;
        }

        /// <summary>
        /// Uses the updated InvokeYoutubeVideoSearchAsync2 method to retrieve videos
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="MaxVideos"></param>
        /// <returns></returns>
        public async Task<List<Video>> GetVideosFromCreator(Creator creator, int MaxVideos = 5)
        {
            var cachedResults = this.GetCreatorVideos(creator);

            if (cachedResults.Count > 0)
            {
                return cachedResults;
            }

            //you youtubeRepo to get first X videos from this creator
            var response = new List<Video>();
            var result = await youTubeRepo.InvokeYoutubeVideoSearchAsync2(creator, MaxVideos);

            if (null == result)
            {
                return response;
            }

            foreach (var item in result)
            {
                var video = new Video(item, creator);
                response.Add(video);
            }

            this.AddCreatorVideos(creator, response);
            return response;
        }

        private List<Video> GetCreatorVideos(Creator creator)
        {
            //check local dir for creator.handle with spaces removed .json file, if exists, then parse to Videos format and return
            var creatorExpectedPath = $"videos/{creator.ChannelName}.json";
            var response = new List<Video>();

            if (!System.IO.File.Exists(creatorExpectedPath))
            {
                return response;
            }

            var jsonContent = System.IO.File.ReadAllText(creatorExpectedPath);

            // Deserialize the JSON into a List<Video>
            response = JsonSerializer.Deserialize<List<Video>>(jsonContent);

            return response;
        }

        private void AddCreatorVideos(Creator creator, List<Video> videos)
        {
            var creatorExpectedPath = $"videos/{creator.ChannelName}.json";

            if (!System.IO.File.Exists(creatorExpectedPath))
            {
                //create file
                //test for videos dir and if missing then create
                if (!System.IO.Directory.Exists("videos"))
                {
                    System.IO.Directory.CreateDirectory("videos");
                }

                System.IO.File.WriteAllText(creatorExpectedPath, "");
                Console.WriteLine($"Created new video list for {creator.ChannelName}");
            }
            else
            {
                //file already existed, we should read it, and then merge our new results in, and select out the uniques by ID
            }

            //we need to use this property when serializing WriteIndented = true
            var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(videos, serializerOptions);

            System.IO.File.WriteAllText(creatorExpectedPath, jsonContent);
        }

        public List<Video> GetVideos()
        {
            //get all source files from the videos directory
            var videoFiles = System.IO.Directory.GetFiles("videos");

            var response = new List<Video>();

            foreach (var videoFile in videoFiles)
            {
                var jsonContent = System.IO.File.ReadAllText(videoFile);

                // Deserialize the JSON into a List<Video>
                var creatorVideos = JsonSerializer.Deserialize<List<Video>>(jsonContent);

                response.AddRange(creatorVideos);
            }

            return response;
        }

        public List<Video> SetNewVideoFromCreator(Video video, Creator creator) => throw new NotImplementedException();

        public void DeleteVideo(Video video) => throw new NotImplementedException();

        public void DeleteVideo(Video video, Creator creator) => throw new NotImplementedException();
    }
}