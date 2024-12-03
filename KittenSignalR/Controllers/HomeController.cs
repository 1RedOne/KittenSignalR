using KittenSignalR.Hubs;
using KittenSignalR.Models;
using Medallion.Shell;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Windows.ApplicationModel.UserDataTasks.DataProvider;

namespace KittenSignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private int executionCount = 0;
        private Timer _timer;
        private readonly IYouTubeRepo _youtubeRepo;
        private readonly ICreatorSourceManager _creatorSourceManager;
        private readonly IVideoSourceManager _videoSourceManager;
        private List<Creator> creators;
        private readonly IHubContext<ChatHub> _hubContext;

        public HomeController(ILogger<HomeController> logger, IHubContext<ChatHub> hubContext, IYouTubeRepo youTubeRepo, ICreatorSourceManager creatorSourceManager, IVideoSourceManager videoSourceManager)
        {
            _logger = logger;
            _hubContext = hubContext;
            _youtubeRepo = youTubeRepo;
            _creatorSourceManager = creatorSourceManager;
            _videoSourceManager = videoSourceManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Ham()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public JsonResult PostTest()
        {
            string output = "Completed";
            return Json(output);
        }

        [HttpGet]
        public IActionResult YoutubeDownload()
        {
            return View();
        }

        [Route("/creators")]
        [HttpGet]
        public IActionResult GetCreators()
        {
            //get creator by handle from list

            var creatorList = _creatorSourceManager
                .GetCreators();

            return View(creatorList);
        }

        [Route("/videos")]
        [HttpGet]
        public IActionResult GetVideos()
        {
            //get creator by handle from list

            var videoList = _videoSourceManager
                .GetVideos();

            return View(videoList);
        }

        [Route("/{creatorHandle}")]
        [HttpGet]
        public IActionResult GetByCreatorHandle(string creatorHandle)
        {
            var creator = this.GetByCreatorHandleOrId(creatorHandle);

            if (creator == null)
            {
                return NotFound();
            }

            return View(creator);
        }

        private Creator GetByCreatorHandleOrId(string creatorHandle)
        {
            var creatorList = _creatorSourceManager
                .GetCreators();
            //do a case insensitive search
            var creator = creatorList
                .FindAll(c => c.ChannelName.Equals(creatorHandle, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            var creatorHandleResponse = creatorList
                .FindAll(c => c.ChannelId.Equals(creatorHandle, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault();

            return creator ?? creatorHandleResponse;
        }

        public async Task<IActionResult> GetVideosByCreatorHandle(string handle)
        {
            var videos = new List<Video>();
            var creator = this.GetByCreatorHandleOrId(handle);

            await this.EnsureThatCreatorPlaylistUriIsNotNull(creator);

            var videoList = await _videoSourceManager.GetVideosFromCreator(creator, 5);
            if (videoList != null)
            {
                return PartialView("_VideosByCreator", videoList);
            }

            for (int i = 0; i < 5; i++)
            {
                var vidThumb = await GetVideoThumbnailByHandleAndUri(handle, $"uri{i}");
                videos.Add(new Video(null, creator) { CreatorId = handle, VideoThumbnail = vidThumb, VideoUrl = vidThumb, VideoTitle = $"Video {i} by {handle}", VideoDescription = "This is a video description", VideoLength = 60 });
            }

            return PartialView("_VideosByCreator", videos);
        }

        [HttpPut]
        [Route("/{creatorHandle}/delete")]
        public async Task<IActionResult> Delete(string creatorHandle)
        {
            var creator = this.GetByCreatorHandleOrId(creatorHandle);

            if (creator == null)
            {
                return NotFound();
            }

            _creatorSourceManager.DeleteCreator(creator);

            return new OkObjectResult($"deleted {creator.ChannelName}");
        }

        [HttpPut]
        [Route("/{creatorHandle}/refresh")]
        public async Task<IActionResult> Refresh(string creatorHandle)
        {
            var creators = await this.YoutubeLookup(creatorHandle, forceRefresh: true);
            var creator = creators.FirstOrDefault();

            if (creator == null)
            {
                return NotFound();
            }

            return new OkObjectResult($"Refreshed details for {creator.ChannelName}");
        }

        [HttpGet]
        [Route("api/home/GetVideoThumbnailByHandleAndUri")]
        public async Task<string> GetVideoThumbnailByHandleAndUri(string handle, string videoUri)
        {
            // Define the images folder and construct the file path
            string imagesFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images");
            string fileName = $"{handle}_{Uri.EscapeDataString(videoUri)}.png";
            string filePath = Path.Combine(imagesFolder, fileName);

            // Check if the file already exists
            if (System.IO.File.Exists(filePath))
            {
                return $"/images/{fileName}"; // Return the relative path to the existing thumbnail
            }

            // Generate a new gradient image if the file doesn't exist
            Directory.CreateDirectory(imagesFolder); // Ensure the folder exists
            GenerateGradientImage(filePath);

            return $"/images/{fileName}"; // Return the relative path to the new thumbnail
        }

        [Route("api/home/autocomplete")]
        [HttpGet]
        public async Task<IActionResult> Autocomplete(string query)
        {
            if (string.IsNullOrEmpty(query) || query.Length < 3)
            {
                return Ok(new List<Creator>()); // Return an empty list for empty queries
            }

            var creators = _creatorSourceManager.GetCreators();
            // Filter the list for autocomplete suggestions (case-insensitive)
            var results = creators
                .Where(c => c.ChannelName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            //when results list is empty, console out that we need to look this up on youtube's api
            if (results.Count == 0)
            {
                Console.WriteLine($"No results found for {query}.  Look this up on YouTube's API.");

                results = await this.YoutubeLookup(query);
            }

            return Ok(results); // Return the filtered list as JSON
        }

        private async Task<List<Creator>> YoutubeLookup(string query, bool forceRefresh = false)
        {
            //lookup on Youtube
            var ytResponse = await this._youtubeRepo.InvokeYoutubeCreatorSearchAsync(query);

            if (null == ytResponse)
            {
                return new List<Creator>();
            }

            //if results come back, cast to our Creator type, add to our JSON list so long as does not conflict
            var creator = new Creator(ytResponse);

            //this.creators.Add(creator);
            if (forceRefresh)
            {
                _creatorSourceManager.RefreshCreator(creator);
            }
            else
            {
                _ = _creatorSourceManager.SetNewCreator(creator);
            }

            var retrnList = new List<Creator>() { creator };
            return retrnList;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(List<IFormFile> files, int FileCount)
        {
            long size = files.Sum(f => f.Length);
            string message;
            for (int i = 1; i <= FileCount; i++)
            {
                message = $"Processing file {i} of {FileCount}";
                //await _hubContext.Clients.All.SendAsync("ReceiveMessage", "Server", message);
                await _hubContext.Clients.All.SendAsync("ProgressUpdate", "Server", message);
                Random rnd = new Random();
                int number = rnd.Next(3, 11);

                _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(1));

                //do something cool

                await Task.Delay(500);
            }

            // Process uploaded files
            // Don't rely on or trust the FileName property without validation.

            return Ok(new { count = files.Count, size });
        }

        [HttpPost]
        public async Task<IActionResult> YoutubeDownload(string videolist)
        {
            string[] videos = videolist.Split(',');
            string message;
            Debug.WriteLine("new YouTubeDL Post received...");
            int i = 0;
            foreach (string video in videos)
            {
                i++;
                message = $"processing file #{i} [{video}] of {videos.Count()}";
                await _hubContext.Clients.All.SendAsync("ProgressUpdate", "Server", message);
                Random rnd = new Random();
                int number = rnd.Next(3, 11);

                _timer = new Timer(DoWork, null, TimeSpan.Zero,
                TimeSpan.FromSeconds(1));
                string filePath = "/youtubeDLs/";

                var command = Command.Run("bash", "-c", "/usr/local/bin/youtube-dl", "-o", $"\"{filePath}%(title)s.%(ext)s\"", video);

                var videoGetter = await VideoGetter(video);

                await _hubContext.Clients.All.SendAsync("ProgressUpdate", "Server", $"[{videoGetter}]");
                await Task.Delay(500);
            }

            return Ok(new { count = videos.Count() });
        }

        private void GenerateGradientImage(string filePath)
        {
            Random rand = new Random();

            // Random colors for the gradient
            var color1 = System.Drawing.Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            var color2 = System.Drawing.Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));

            int width = 300;  // Thumbnail width
            int height = 200; // Thumbnail height

            using (var bitmap = new System.Drawing.Bitmap(width, height))
            using (var graphics = System.Drawing.Graphics.FromImage(bitmap))
            using (var brush = new System.Drawing.Drawing2D.LinearGradientBrush(
                new System.Drawing.Rectangle(0, 0, width, height),
                color1,
                color2,
                45F)) // Gradient angle
            {
                graphics.FillRectangle(brush, 0, 0, width, height);
                bitmap.Save(filePath, System.Drawing.Imaging.ImageFormat.Png);
            }
        }

        private void DoWork(object state)
        {
            executionCount++;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private async Task<string> VideoGetter(string videoURL)
        {
            string cleanURL = videoURL.Replace("\r\n", string.Empty).Replace("\n", string.Empty);
            string filePath = "/youtubeDLs/";
            ProcessStartInfo procStartInfo = new ProcessStartInfo
            {
                FileName = "/bin/bash", // -c /usr/local/bin/youtube-dl"", video);
                Arguments = $"-c \"/usr/local/bin/youtube-dl --no-progress {cleanURL}\"",
                //Arguments = args,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                WorkingDirectory = filePath,
                CreateNoWindow = true
            };
            //procStartInfo.ArgumentList.Add("-c");
            //procStartInfo.ArgumentList.Add("/usr/local/bin/youtube-dl");
            //procStartInfo.ArgumentList.Add("--version");
            //procStartInfo.ArgumentList.Add("-o");
            //procStartInfo.ArgumentList.Add($"\"{ filePath}%(title)s.%(ext)s\"");
            //procStartInfo.ArgumentList.Add(videoURL);

            Process proc = new Process();
            proc.StartInfo = procStartInfo;
            proc.Start();
            proc.WaitForExit();
            // Get the output into a string
            return $"{proc.StandardOutput.ReadToEnd()} - {proc.StandardError.ReadToEnd()}";
        }

        private async Task EnsureThatCreatorPlaylistUriIsNotNull(Creator creator)
        {
            //if creator.playlistUri is null we need to refresh the creator object to populate this
            if (null == creator.PlaylistUri)
            {
                var response = await this.YoutubeLookup(creator.ChannelId, forceRefresh: true);

                if (response == null)
                {
                    throw new Exception("Failed to retrieve info");
                }

                creator = response.FirstOrDefault();

                Console.WriteLine($"Refresh completed for {creator.ChannelName} and playlist {creator.PlaylistUri}");
            }
            else
            {
                Console.WriteLine($"No need to refresh URI, for {creator.ChannelName} and playlist {creator.PlaylistUri}");
            }

            return;
        }
    }
}