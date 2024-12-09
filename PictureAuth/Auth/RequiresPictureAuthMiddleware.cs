using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.IO;
using System;
using System.Net.Mime;
using System.Threading.Tasks;
using System.Text.Json;
using PictureAuth.Models;
using PictureAuth.Repo;

namespace PictureAuth.Auth
{
    public class RequiresPictureAuthMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequiresPictureAuthMiddleware> _logger;
        private readonly FoxUserManager _foxUserManager;

        public RequiresPictureAuthMiddleware(RequestDelegate next, FoxUserManager foxUserManager, ILogger<RequiresPictureAuthMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _foxUserManager = foxUserManager;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            this._logger?.LogInformation("RequiresPictureAuthMiddleware executing");
            // Check if the endpoint has the RequireAuthenticationAttribute
            var endpoint = context.GetEndpoint();

            //using var reader = new StreamReader(context.Request.Body);
            //var body = reader.ReadToEndAsync().Result;
            //this._logger.LogInformation(body);

            //attempt to deseerialize to RegisterDto
            //var k = JsonSerializer.Deserialize<RegisterDto>(body);

            var requiresAuth = endpoint?.Metadata.GetMetadata<RequiresPictureAuthAttribute>() != null;
            this._logger?.LogInformation($"Endpoint called was {endpoint?.DisplayName}");
            this._logger?.LogInformation("RequiresPictureAuthMiddleware executing for protected endpoint");

            if (requiresAuth)
            {
                // Check for a cookie or other authentication logic
                var userCookie = context.Request.Cookies["auth_cookie"];
                this._logger?.LogInformation($"Got cookie value of {userCookie}");

                if (string.IsNullOrEmpty(userCookie))
                {
                    this._logger?.LogInformation($"Cookie Value invalid, rejecting with 401");
                    // If not authenticated, return 401 Unauthorized
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Unauthorized");
                    return;
                }

                var user = this._foxUserManager.GetUserFromCookie(userCookie);
                this._logger?.LogInformation($"Cookie Value valid, user is {user.Name} granting access");

                // Optionally, validate the cookie value (e.g., JWT or custom token)
                // Add user info to context if necessary
            }
            else
            {
                this._logger?.LogInformation($"Endpoint unprotected, continue...");
            }

            //this._logger?.LogInformation("[RequiresPictureAuthMiddleware]");
            // Call the next middleware
            await _next(context);
        }
    }
}