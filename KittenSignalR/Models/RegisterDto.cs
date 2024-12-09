using Microsoft.VisualBasic;
using System;
using System.Text.Json.Serialization;

namespace PictureAuth.Models
{
    public class RegisterDto
    {
        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("dateOfBirth")]
        public string DateOfBirth { get; set; }

        [JsonPropertyName("Id")]
        public int Id { get; set; }

        [JsonPropertyName("AuthToken")]
        public int AuthToken { get; set; }
    }
}