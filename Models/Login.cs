using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClientDirectory.Models
{
    public partial class Login
    {
        public int Id { get; set; }
        public Guid IdClient { get; set; }
        public string Password { get; set; }
        public string Salt { get; set; }
        public string Username { get; set; }

        [JsonIgnore]
        public virtual Client IdClientNavigation { get; set; }
    }
}
