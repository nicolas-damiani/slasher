using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Entities
{
    [Serializable]
    [DataContract]
    public class MatchPlayerStatistic
    {
        [DataMember]
        public int MatchId { get; set; }
        [DataMember]
        public List<Tuple<User, bool>> userList { get; set; }

        public MatchPlayerStatistic()
        {
            userList = new List<Tuple<User, bool>>();
        }
    }
}
