using System;
using System.Collections.Generic;
using System.Text;

namespace BlindChatCore
{
    public class Group
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string OwnerEmail { get; set; }

        public string PublicKey { get; set; }
    }
}
