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
        public List<MatchPlayerStatistic> Statistics { get; set; }
        public List<UserScore> UserScores { get; set; }
        private static ServerSystem Instance = null;

        private ServerSystem()
        {
            RegisteredUsers = new List<User>();
            Statistics = new List<MatchPlayerStatistic>();
            UserScores = new List<UserScore>();
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
