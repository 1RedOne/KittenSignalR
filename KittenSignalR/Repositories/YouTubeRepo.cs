using KittenSignalR.Models;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Windows.Web.Http;
using HttpClient = System.Net.Http.HttpClient;
using HttpRequestMessage = System.Net.Http.HttpRequestMessage;

namespace KittenSignalR.Repositories
{
    public class YouTubeRepo : IYouTubeRepo
    {
        //wire up to accept httpclient as well as the api key
        public string ApiKey { get; set; }

        public HttpClient _httpClient { get; }
        public ILogger<IYouTubeRepo> Logger { get; }

        public YouTubeRepo(IOptions<YouTubeRepoOptions> options, HttpClient httpClient, ILogger<IYouTubeRepo> logger)
        {
            //set the api key from the Options, should be titled ApiKey
            this.ApiKey = options.Value.ApiKey;
            this._httpClient = httpClient;
            this.Logger = logger;
        }

        public async Task<YouTubeChannelResult> InvokeYoutubeCreatorSearchAsync(string query)
        {
            var requestUrl = $"https://youtube.googleapis.com/youtube/v3/channels?part=snippet,contentDetails,statistics&forHandle={query}&key={this.ApiKey}";
            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, requestUrl);

            //this style of auth applies to oAuth Tokens only, not simple API Keys
            //#request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            var l = response.StatusCode;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();
            var json = JsonConvert.DeserializeObject<YouTubeSearchResponse>(content);

            if (null == json.items || json.items.Count == 0)
            {
                return null;
            }

            //get .snippet property and cast to
            var channelResult = new YouTubeChannelResult(json.items[0]);
            return channelResult;
        }

        public async Task<YouTubeVideoResult> InvokeYoutubeVideoSearchAsync(string creator, int maxCount = 5)
        {
            var requestUrl = $"https://youtube.googleapis.com/youtube/v3/search?key={this.ApiKey}&forHandle={creator}&part=snippet,id&order=date&maxResults={maxCount}";
            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, requestUrl);

            //this style of auth applies to oAuth Tokens only, not simple API Keys
            //#request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            var l = response.StatusCode;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            //returns the first maxCount videos from the specified creators 'Uploads' playlist
            var json = JsonConvert.DeserializeObject<YouTubeSearchResponse>(content);

            if (null == json.items || json.items.Count == 0)
            {
                return null;
            }

            //there will be multiple videos contained within the snippet
            //json.items[0].snippet
            var videoResult = new YouTubeVideoResult(null);
            return videoResult;
        }

        /// <summary>
        /// Uses the playlist lookup technique which is much faster
        /// </summary>
        /// <param name="creator"></param>
        /// <param name="maxCount"></param>
        /// <returns></returns>
        public async Task<List<YouTubeVideoResult>> InvokeYoutubeVideoSearchAsync2(Creator creator, int maxCount = 5)
        {
            //use creator.ChannelId to get the uploads playlist if present otherwise retrieve and update the creaotr.playlistUri
            if (null == creator.PlaylistUri)
            {
                //todo add code to retrieve here
            }

            var requestUrl = $"https://youtube.googleapis.com/youtube/v3/playlistItems?part=contentDetails%2Csnippet&maxResults=5&playlistId={creator.PlaylistUri}&key={this.ApiKey}";
            //var requestUrl = $"https://youtube.googleapis.com/youtube/v3/search?key={this.ApiKey}&forHandle={creator}&part=snippet,id&order=date&maxResults={maxCount}";
            var request = new HttpRequestMessage(System.Net.Http.HttpMethod.Get, requestUrl);

            //this style of auth applies to oAuth Tokens only, not simple API Keys
            //#request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ApiKey);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var response = await _httpClient.SendAsync(request);
            var l = response.StatusCode;
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            //returns the first maxCount videos from the specified creators 'Uploads' playlist
            var json = JsonConvert.DeserializeObject<YouTubeSearchResponse>(content);

            if (null == json.items || json.items.Count == 0)
            {
                return null;
            }

            //cast items to a list of Video entities
            var list = new List<YouTubeVideoResult>();
            foreach (var item in json.items)
            {
                list.Add(new YouTubeVideoResult(item));
            }

            return list;
        }

        //public async Task<YouTubeVideoResult> InvokeYoutubeCreatorSearchAsync(string query)
        //{
        //}
    }

    public class YouTubeRepoOptions
    {
        public string ApiKey { get; set; }
    }
}