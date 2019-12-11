using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using KittenSignalR.Models;
using Microsoft.AspNetCore.Http;
using System.IO;
using System.Threading;
using KittenSignalR.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace KittenSignalR.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private int executionCount = 0;
        private Timer _timer;
        private readonly IHubContext<ChatHub> _hubContext;

        public HomeController(ILogger<HomeController> logger, IHubContext<ChatHub> hubContext)
        {
            _logger = logger;
            _hubContext = hubContext;
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
        public IActionResult Upload()
        {
            return View();

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

        private void DoWork(object state)
        {
            executionCount++;

            _logger.LogInformation(
                "Timed Hosted Service is working. Count: {Count}", executionCount);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
