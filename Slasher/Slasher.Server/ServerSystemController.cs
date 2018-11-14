using Exceptions;
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
    public class ServerSystemController : MarshalByRefObject
    {
        private ServerSystem system;

        public ServerSystemController()
        {
        }

        public void AddUserToSystem(User user)
        {
            var registeredUsers = ServerSystem.GetInstance().RegisteredUsers;
            if (!registeredUsers.Contains(user))
            {
                registeredUsers.Add(user);
            }
            else
                throw new ServerSystemException("Ya existe un usuario con ese nombre.");
        }

        public List<User> GetRegisteredUsers()
        {
            List<User> users = ServerSystem.GetInstance().RegisteredUsers;
            return users;
        }

        public void DeleteUser(string username)
        {
            User user = new User(username);
            var registeredUsers = ServerSystem.GetInstance().RegisteredUsers;
            if (registeredUsers.Contains(user))
            {
                registeredUsers.Remove(user);
            }
            else
                throw new ServerSystemException("No existe un usuario con ese nombre");

        }

        public void ModifyUser(string oldname, string newName)
        {
            User user = new User(oldname);
            var registeredUsers = ServerSystem.GetInstance().RegisteredUsers;
            if (registeredUsers.Contains(user))
            {
                User userInServer = registeredUsers.FirstOrDefault(x => x.NickName.Equals(oldname));
                userInServer.NickName = newName;
            }else
            {
                throw new ServerSystemException("No existe un usuario con ese nombre.");
            }
        }

        public void AddStatistic(MatchPlayerStatistic statistic)
        {
            ServerSystem.GetInstance().Statistics.Add(statistic);
        }

        public List<MatchPlayerStatistic> GetUserStatistics()
        {
            return ServerSystem.GetInstance().Statistics;
        }

        public void AddScores(List<UserScore> userScores)
        {
            foreach (UserScore userScore in userScores)
            {
                ServerSystem.GetInstance().UserScores.Add(userScore);
            }
        }

        public List<UserScore> GetHighScores()
        {
            List<UserScore> sortedList = ServerSystem.GetInstance().UserScores.OrderByDescending(o => o.Score).ToList();
            List<UserScore> returnList = new List<UserScore>();
            for (int i = 0; i < Math.Min(sortedList.Count, 10); i++)
            {
                returnList.Add(sortedList[i]);
            }
            return returnList;
        }

        public User GetUserInServer(User user)
        {
            var registeredUsers = ServerSystem.GetInstance().RegisteredUsers;
            if (registeredUsers.Contains(user))
            {
                User userInServer = registeredUsers.FirstOrDefault(x => x.NickName.Equals(user.NickName));
                return userInServer;
            }
            return null;
        }
    }
}
