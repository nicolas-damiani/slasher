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
    public class User
    {
        [DataMember]
        public string Id { get; set; }
        [DataMember]
        public string NickName { get; set; }
        [DataMember]
        public string Avatar { get; set; }
        [DataMember]
        public Character Character { get; set; }
        [DataMember]
        public bool Connected { get; set; }
        [DataMember]
        public int Turn { get; set; }
        [DataMember]
        public Dictionary<string, CharacterType> CharacterTypesCommand;
        [DataMember]
        public int CurrentKills { get; set; }
        [DataMember]
        public Tuple<int, DateTime, CharacterType> RecordInfo { get; set; }


        public User(string nickname, string avatar)
        {
            NickName = nickname;
            Avatar = avatar;
            Connected = true;
            Turn = 0;
            CurrentKills = 0;
            Id = Guid.NewGuid().ToString();
            CharacterTypesCommand = new Dictionary<string, CharacterType>()
            {
                { "monstruo", CharacterType.MONSTER },
                { "sobreviviente", CharacterType.SURVIVOR},
            };
        }

        public User(string nickname)
        {
            NickName = nickname;
            Connected = true;
            Turn = 0;
            CurrentKills = 0;
            Id = Guid.NewGuid().ToString();
            CharacterTypesCommand = new Dictionary<string, CharacterType>()
            {
                { "monstruo", CharacterType.MONSTER },
                { "sobreviviente", CharacterType.SURVIVOR},
            };
        }

        public User()
        {
            Connected = true;
            Turn = 0;
            Id = Guid.NewGuid().ToString();
            CharacterTypesCommand = new Dictionary<string, CharacterType>()
            {
                { "monstruo", CharacterType.MONSTER },
                { "sobreviviente", CharacterType.SURVIVOR},
            };
        }

        public override bool Equals(object objectToCompare)
        {
            if (objectToCompare == null || !objectToCompare.GetType().Equals(this.GetType()))
                return false;
            else
            {
                User compareUser = (User)objectToCompare;
                return this.NickName.Equals(compareUser.NickName);
            }
        }

        public void SetCharacterType(CharacterType type)
        {
            Character = new Character(type);
        }
    }
}
