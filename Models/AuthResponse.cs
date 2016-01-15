using System;

namespace ClientDirectory.Models
{
    public class AuthResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string AuthToken { get; set; }
        public bool CanManageClients { get; set; }
        public bool CanEditOwnInfo { get; set; }
    }
}