using Slasher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Server
{
    [Serializable]
    public class ServerSystem
    {
        public List <User> RegisteredUsers { get; set; }
        private static ServerSystem Instance = null;

        private ServerSystem()
        {
            RegisteredUsers = new List<User>();
        }

        public static ServerSystem GetInstance()
        {
            if (Instance == null)
            {
                Instance = new ServerSystem();
            }
            return Instance;
        }
    }
}
