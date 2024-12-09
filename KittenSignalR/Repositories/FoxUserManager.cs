using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using PictureAuth.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PictureAuth.Repo
{
    public class FoxUserManager
    {
        private ILogger<FoxUserManager> logger;
        private List<RegisterDto> userList;
        private string userExpectedPath = $"userRecord/userRecordList.json";

        public FoxUserManager(ILogger<FoxUserManager> logger)
        {
            this.logger = logger;
            this.userList = this.WarmupUserList();
        }

        private List<RegisterDto>? WarmupUserList()
        {
            this.logger?.BeginScope("[JsonUserStoreSetup]");
            this.EnsureUserListReady();

            this.logger?.LogInformation("Loading user list");
            var jsonContent = System.IO.File.ReadAllText(this.userExpectedPath);

            if (string.IsNullOrWhiteSpace(jsonContent))
            {
                return new List<RegisterDto>();
            }

            // Deserialize the JSON into a List<Channel>
            var warmingUserlist = JsonSerializer.Deserialize<List<RegisterDto>>(jsonContent);
            this.logger?.LogInformation($"Online with ${warmingUserlist.Count} users");

            return warmingUserlist;
        }

        private void EnsureUserListReady()
        {
            this.logger?.LogInformation("Ensuring user list path is ready");
            if (!System.IO.File.Exists(userExpectedPath))
            {
                if (!System.IO.Directory.Exists("userRecord"))
                {
                    this.logger?.LogInformation("Need to create source directory");

                    System.IO.Directory.CreateDirectory("userRecord");

                    this.logger?.LogInformation("Created source directory");
                }

                var list = new List<RegisterDto>();

                WriteUserList(userExpectedPath, list);
            }

            this.logger?.LogInformation("User list path is ready");
        }

        private void WriteUserList(string userExpectedPath, List<RegisterDto> list)
        {
            var serializerOptions = new JsonSerializerOptions { WriteIndented = true };
            var jsonContent = JsonSerializer.Serialize(list, serializerOptions);

            System.IO.File.WriteAllText(userExpectedPath, jsonContent);
        }

        private void RefreshUserList()
        {
            this.WriteUserList(this.userExpectedPath, this.userList);
            this.logger?.LogInformation($"[RefreshUserList], list now has {this.userList.Count()} members");
        }

        public Task<FoxIdentityResult> CreateAsync(RegisterDto user, CancellationToken cancellationToken)
        {
            //check if user exists already
            if (userList.Any(x => x.Name == user.Name))
            {
                this.logger.LogCritical("User already existed, failing");
                return Task.FromResult(new FoxIdentityResult { ErrorMessage = "User already existed" });
            }

            user.Id = this.userList.Count();

            //get random number between 1 and 512

            user.AuthToken = new Random().Next(int.MinValue, int.MaxValue);
            this.logger?.LogInformation($"[CreateUser], list had {this.userList.Count()} members, adding one");
            this.userList.Add(user);
            RefreshUserList();

            return Task.FromResult(new FoxIdentityResult());
        }

        public RegisterDto GetUserFromCookie(string authToken)
        {
            var asInt = int.Parse(authToken);
            return this.GetUserFromCookie(asInt);
        }

        public RegisterDto GetUserFromCookie(int authToken)
        {
            if (this.userList.Any(x => x.AuthToken == authToken))
            {
                return this.userList.FirstOrDefault(x => x.AuthToken == authToken);
            }

            return null;
        }

        public Task<FoxIdentityResult> DeleteAsync(RegisterDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public Task<RegisterDto> FindByIdAsync(string userId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public RegisterDto FindByNameAsync(RegisterDto loginAttempt, CancellationToken cancellationToken)
        {
            if (this.userList.Any(x => x.Name == loginAttempt.Name))
            {
                return this.userList.FirstOrDefault(x => x.Name == loginAttempt.Name);
            }

            return null;
        }

        public Task<string> GetNormalizedUserNameAsync(RegisterDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserIdAsync(RegisterDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<string> GetUserNameAsync(RegisterDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetNormalizedUserNameAsync(RegisterDto user, string normalizedName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task SetUserNameAsync(RegisterDto user, string userName, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<FoxIdentityResult> UpdateAsync(RegisterDto user, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        internal List<RegisterDto> ListUsers()
        {
            return this.userList;
        }
    }

    internal class OkFoxIdentityResult : FoxIdentityResult
    {
    }

    internal class FailFoxIdentityResult : FoxIdentityResult
    {
        public string FailureReason { get; set; }

        public FailFoxIdentityResult(string errorCause) => this.FailureReason = errorCause;
    }

    public class FoxIdentityResult
    {
        public bool LoginAllowed { get; set; }
        public string? ErrorMessage { get; set; }
    }
}