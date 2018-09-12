using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Entities
{
    public class Character
    {
        private const int MONSTER_ATTACK = 10;
        private const int SURVIVOR_ATTACK = 5;
        private const int MONSTER_LIFE = 100;
        private const int SURVIVOR_LIFE = 20;

        public int Life { get; set; }
        public int PowerAttack { get; set; }
        public CharacterType Type { get; set; }

        public Character(CharacterType type)
        {
            if (type == CharacterType.MONSTER)
            {
                this.Life = MONSTER_LIFE;
                this.PowerAttack = MONSTER_ATTACK;
                this.Type = CharacterType.MONSTER;
            }
            else
            {
                this.Life = SURVIVOR_LIFE;
                this.PowerAttack = SURVIVOR_ATTACK;
                this.Type = CharacterType.SURVIVOR;
            }
        }
    }
}
