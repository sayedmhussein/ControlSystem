﻿using System;
using Newtonsoft.Json;

namespace Sayed.MySystem.ClientService.Models
{
    public class Api
    {
        [JsonProperty("Base")]
        public Uri Base { get; set; }

        [JsonProperty("Version")]
        public string Version { get; set; }

        [JsonProperty("Login")]
        public string Login { get; set; }

        [JsonProperty("Token")]
        public string Token { get; set; }

        [JsonProperty("Logout")]
        public string Logout { get; set; }
    }
}
