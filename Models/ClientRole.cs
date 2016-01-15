using System;
using System.Collections.Generic;

namespace ClientDirectory.Models
{
    public partial class ClientRole
    {
        public ClientRole()
        {
            Client = new HashSet<Client>();
        }

        public int Id { get; set; }
        public string Description { get; set; }

        public virtual ICollection<Client> Client { get; set; }
    }
}
