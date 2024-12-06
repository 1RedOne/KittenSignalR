using KittenSignalR.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.DotNet.Scaffolding.Shared;
using Microsoft.Extensions.Logging;
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

        private ILogger<VideoSourceManager> logger;

        public List<Video> cachedVideos { get; private set; }

        public VideoSourceManager(IYouTubeRepo youTubeRepo, ILogger<VideoSourceManager> logger)
        {
            this.creatorList = GetCreators();
            this.youTubeRepo = youTubeRepo;
            this.logger = logger;
        }

        public async Task<List<Video>> GetVideos(Creator creator)
        {
            //add console logigng that we are loading the list
            logger?.LogInformation($"Loading video list for {creator.ChannelName}");

            //check if creator file exists
            var creatorExpectedPath = $"videos/{creator.ChannelName}.json";

            if (!System.IO.File.Exists(creatorExpectedPath))
            {
                logger?.LogWarning($"No video list found for {creator.ChannelName} at {creatorExpectedPath}");

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
            logger?.LogInformation("Loading creator list");
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
        public async Task<List<Video>> GetVideosFromCreator(Creator creator, int MaxVideos = 5, int AdditionalRequestedVideoCount = 0)
        {
            var cachedResults = this.GetCreatorVideos(creator);

            if (cachedResults.Count > 0 && AdditionalRequestedVideoCount < 1)
            {
                logger?.LogInformation($"found [{cachedResults.Count}] videos for {creator.ChannelName}");
                return cachedResults;
            }

            if (AdditionalRequestedVideoCount > 0)
            {
                if (MaxVideos < cachedResults.Count)
                {
                    MaxVideos = cachedResults.Count + AdditionalRequestedVideoCount;
                }
                else
                {
                    MaxVideos += AdditionalRequestedVideoCount;
                }

                logger?.LogInformation($"found [{cachedResults.Count}] videos for {creator.ChannelName}, but we need [{MaxVideos}], contacting youtube!");
            }
            else
            {
                logger?.LogInformation($"found no cached videos for {creator.ChannelName}, contacting youtube!");
            }

            //you youtubeRepo to get first X videos from this creator
            var response = new List<Video>();
            var result = await youTubeRepo.InvokeYoutubeVideoSearchAsync2(creator, MaxVideos);

            if (null == result)
            {
                logger?.LogWarning($"Received no videos for {creator.ChannelName}");
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
                logger?.LogInformation($"Created new video list for {creator.ChannelName}");
            }
            else
            {
                logger?.LogInformation($"Found previous video list for {creator.ChannelName}");
                var existingJsonContent = System.IO.File.ReadAllText(creatorExpectedPath);

                // Deserialize the JSON into a List<Video>
                var existingVideos = JsonSerializer.Deserialize<List<Video>>(existingJsonContent);
                logger?.LogInformation($"Previous list had [{existingVideos.Count}] videos");
                existingVideos.AddRange(videos);

                logger?.LogInformation($"Present list has [{existingVideos.Count}] videos, deduping...");
                var dedupe1 = existingVideos.DistinctBy(x => x.VideoId).ToList();
                var dedupe2 = existingVideos.Distinct().ToList();
                var deduper3 = existingVideos.GroupBy(t => t.VideoId).Select(grp => grp.First()).ToList();

                logger?.LogInformation($"new merged count of videos is [{dedupe1.Count}]");
                videos = dedupe1;
            }

            //we need to use this property when serializing WriteIndented = true
            var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(videos, serializerOptions);

            logger?.LogInformation($"Commiting new video list count of [{videos.Count}]");

            System.IO.File.WriteAllText(creatorExpectedPath, jsonContent);
        }

        public List<Video> GetVideos()
        {
            //get all source files from the videos directory
            var videoFiles = System.IO.Directory.GetFiles("videos");

            var response = new List<Video>();

            logger?.LogInformation($"Found [{videoFiles.Count()}] video files");

            foreach (var videoFile in videoFiles)
            {
                var jsonContent = System.IO.File.ReadAllText(videoFile);

                // Deserialize the JSON into a List<Video>
                var creatorVideos = JsonSerializer.Deserialize<List<Video>>(jsonContent);

                response.AddRange(creatorVideos);
            }

            logger?.LogInformation($"Found [{response.Count()}] videos");

            this.cachedVideos = response;
            return response;
        }

        public Video GetVideo(string videoId)
        {
            if (null == cachedVideos)
            {
                _=this.GetVideos();
            }
            var matchingVideo = this.cachedVideos.Where(x => x.VideoId == videoId).FirstOrDefault();

            return matchingVideo;
        }

        public List<Video> SetNewVideoFromCreator(Video video, Creator creator) => throw new NotImplementedException();

        public void DeleteVideo(Video video) => throw new NotImplementedException();

        public void DeleteVideo(Video video, Creator creator) => throw new NotImplementedException();
    }
}