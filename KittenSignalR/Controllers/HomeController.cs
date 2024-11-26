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
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace KittenSignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private int executionCount = 0;
        private Timer _timer;
        private List<Creator> creators;
        private readonly IHubContext<ChatHub> _hubContext;

        public HomeController(ILogger<HomeController> logger, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
             var jsonContent = System.IO.File.ReadAllText("creatorList.json");

            // Deserialize the JSON into a List<Channel>
            this.creators = JsonSerializer.Deserialize<List<Creator>>(jsonContent);

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

        [Route("api/home/autocomplete")]
        [HttpGet]
        public IActionResult Autocomplete(string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return Ok(new List<Creator>()); // Return an empty list for empty queries
            }

            // Filter the list for autocomplete suggestions (case-insensitive)
            var results = this.creators
                .Where(c => c.ChannelName.Contains(query, StringComparison.OrdinalIgnoreCase))
                .ToList();

            return Ok(results); // Return the filtered list as JSON
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
                        

            return Ok(new { count = videos.Count()});
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
    }
}
