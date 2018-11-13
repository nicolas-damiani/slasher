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
    public class UserScore
    {
        [DataMember]
        public DateTime Date { get; set; }
        [DataMember]
        public int Score { get; set; }
        [DataMember]
        public CharacterType CharacterType { get; set; }
        [DataMember]
        public User user { get; set; }
    }
}
