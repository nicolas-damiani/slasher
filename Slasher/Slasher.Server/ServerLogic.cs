using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slasher.Entities;

namespace Slasher.Server
{
    class ServerLogic
    {
        public Match Match { get; set; }

        internal List<User> GetPossibleUsersToAdd()
        {
            throw new NotImplementedException();
        }

        internal void AcceptUser(User user)
        {
            throw new NotImplementedException();
        }

        internal List<User> GetRegisteredUsers()
        {
            throw new NotImplementedException();
        }

        internal List<User> GetConnectedPlayers()
        {
            throw new NotImplementedException();
        }

        internal bool CanStartMatch()
        {
            return !Match.Active;
        }

        internal void StartMatch()
        {
            Match = new Match();
            Match.StartMatch();
        }
    }
}
