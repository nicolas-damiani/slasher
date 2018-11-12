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
    public class Character
    {
        [DataMember]
        public const int MONSTER_ATTACK = 10;
        [DataMember]
        public const int SURVIVOR_ATTACK = 5;
        [DataMember]
        public const int MONSTER_LIFE = 100;
        [DataMember]
        public const int SURVIVOR_LIFE = 20;

        [DataMember]
        public int Life { get; set; }
        [DataMember]
        public int PowerAttack { get; set; }
        [DataMember]
        public CharacterType Type { get; set; }
        [DataMember]
        public bool IsAlive { get; set; }

        public Character(CharacterType type)
        {
            if (type == CharacterType.MONSTER)
            {
                this.Life = MONSTER_LIFE;
                this.PowerAttack = MONSTER_ATTACK;
                this.Type = CharacterType.MONSTER;
                this.IsAlive = true;
            }
            else
            {
                this.Life = SURVIVOR_LIFE;
                this.PowerAttack = SURVIVOR_ATTACK;
                this.Type = CharacterType.SURVIVOR;
                this.IsAlive = true;
            }
        }
    }
}
