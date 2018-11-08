using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Entities
{
    public class User
    {
        public string NickName { get; set; }
        public string Avatar { get; set; }
        public Character Character { get; set; }
        public bool Connected { get; set; }
        public int Turn { get; set; }
        public Dictionary<string, CharacterType> CharacterTypesCommand;

        public User(string nickname, string avatar)
        {
            NickName = nickname;
            Avatar = avatar;
            Connected = true;
            Turn = 0;
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
