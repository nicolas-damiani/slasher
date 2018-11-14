using Slasher.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Slasher.Entities;
using System.Net.Sockets;
using System.Runtime.Remoting;

namespace WCFHosting
{
    public class Service : IService
    {
        public ServerSystemController remoteObject;
        public Service() {
            remoteObject = (ServerSystemController)Activator.GetObject(
                            typeof(ServerSystemController),
                            "tcp://localhost:1234/ServerSystemControllerUri",
                            WellKnownObjectMode.Singleton);
        }

        public void AddUserToSystem(User user)
        {
            remoteObject.AddUserToSystem(user);
        }

        public void DeleteUser(string username)
        {
            remoteObject.DeleteUser(username);
        }

        public List<User> GetRegisteredUsers()
        {
            return remoteObject.GetRegisteredUsers();
        }

        public void ModifyUser(string oldName, string newName)
        {
            remoteObject.ModifyUser(oldName, newName);
        }

        public void AddStatistic(MatchPlayerStatistic statistic)
        {
            remoteObject.AddStatistic(statistic);
        }

        public List<MatchPlayerStatistic> GetUserStatistics()
        {
            return remoteObject.GetUserStatistics();
        }

        public void AddScores(List<UserScore> userScores)
        {
            remoteObject.AddScores(userScores);
        }

        public List<UserScore> GetHighScores()
        {
            return remoteObject.GetHighScores();
        }
    }
}
