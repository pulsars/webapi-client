using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ClientDirectory.Models
{
    public partial class Client
    {
        public Client()
        {
            Login = new HashSet<Login>();
        }

        public Guid Id { get; set; }
        public bool Active { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public int IdClientRole { get; set; }
        public string JobTitle { get; set; }
        public string Location { get; set; }
        public string Name { get; set; }
        public string PhoneNumbers { get; set; }

        [JsonIgnore]
        public virtual ICollection<Login> Login { get; set; }

        [JsonIgnore]
        public virtual ClientRole IdClientRoleNavigation { get; set; }
    }
}
