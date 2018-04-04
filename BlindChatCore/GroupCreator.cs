using Org.BouncyCastle.Crypto.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlindChatCore
{
    public class GroupCreator
    {
        private readonly IAPIServer server;

        public GroupCreator(IAPIServer server)
        {
            this.server = server;
        }


        public void RegisterGroup(Group group)
        {
            server.CreateGroup(group);
        }

        public void AddParticipantsToGroup(List<string> emails, int confirmationCode)
        {
            server.AddParticipantsToGroup(emails, confirmationCode);
        }

    }
}
