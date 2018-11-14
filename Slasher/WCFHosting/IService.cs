using Slasher.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace WCFHosting
{
    [ServiceContract]
    public interface IService
    {

        [OperationContract]
        void AddUserToSystem(User user);

        [OperationContract]
        List<User> GetRegisteredUsers();

        [OperationContract]
        void DeleteUser(string username);

        [OperationContract]
        void ModifyUser(string oldName, string newName);
        
        [OperationContract]
        void AddStatistic(MatchPlayerStatistic statistic);


        [OperationContract]
        List<MatchPlayerStatistic> GetUserStatistics();

        [OperationContract]
        void AddScores(List<UserScore> userScores);

        [OperationContract]
        List<UserScore> GetHighScores();


    }
}
