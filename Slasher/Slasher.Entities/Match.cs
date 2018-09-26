using Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace Slasher.Entities
{
    public class Match
    {
        public List<User> Users { get; set; }
        public User[,] Map { get; set; }
        public bool Active { get; set; }
        public enum Direction { UP, DOWN, LEFT, RIGHT }
        public static Dictionary<string, Direction> MovementCommands;
        private const int FIRST_ROW = 0;
        private const int LAST_ROW = 7;
        private const int FIRST_COL = 0;
        private const int LAST_COL = 7;
        private readonly object lockMap = new object();
        Thread timerThread;
        public List<User> Winners { get; set; }

        public Match()
        {
            MovementCommands = new Dictionary<string, Direction>()
            {
                { "arriba", Direction.UP},
                { "abajo", Direction.DOWN},
                { "izquierda", Direction.LEFT},
                { "derecha", Direction.RIGHT}
            };
        }

        public void StartMatch()
        {
            Users = new List<User>();
            Winners = new List<User>();
            Map = new User[8, 8];
            InitializeMap();
            Active = true;
            timerThread = new Thread(finishMatchByTime);
            timerThread.Start();
        }

        private void finishMatchByTime()
        {
            Thread.Sleep(10000);
            lock (lockMap)
            {
                if (Active)
                {
                    Active = false;
                    IsThereSurvivorsLeft();
                }
            }
        }

        private void IsThereSurvivorsLeft()
        {
            bool survivorsWin = false;
            List<User> aliveUsers = new List<User>();
            foreach (User user in Users)
            {
                if (user.Character.IsAlive && user.Character.Type == CharacterType.SURVIVOR)
                {
                    survivorsWin = true;
                    Winners.Add(user);
                }
            }
            if (survivorsWin)
                throw new SurvivorsWinException();
        }

        private void InitializeMap()
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Map.SetValue(null, i, j);
                }
            }
        }

        public bool AddUserToMatch(User user)
        {
            return SetUserRandomPosition(user);
        }

        private bool SetUserRandomPosition(User user)
        {
            Random random = new Random();
            bool assigned = false;
            while (!assigned)
            {
                int row = random.Next(0, 7);
                int col = random.Next(0, 7);
                lock (lockMap)
                {
                    if (Map[row, col] == null)
                    {
                        Tuple<int, int> position = new Tuple<int, int>(row, col);
                        SetUserPosition(user, position);
                        assigned = true;
                    }
                }
            }
            return true;
        }

        private void SetUserPosition(User user, Tuple<int, int> position)
        {
            Map[position.Item1, position.Item2] = user;
        }

        public string MovePlayer(User user, Direction direction)
        {
            lock (lockMap)
            {
                if (user.Turn < 2)
                {
                    Tuple<int, int> position = FindUserPosition(user);
                    MoveInsideBounds(position, direction);
                    IsEmptySlot(user, position, direction);
                    MovePlayerTile(user, position, direction);
                    user.Turn = user.Turn + 1;
                    CheckFinishedMatch();
                    ProcessAllTurns();
                    return GetCloseUsersList(user);
                }
                else
                {
                    throw new UserTurnLimitException();
                }
            }
        }

        private void CheckFinishedMatch()
        {
            if (BothTypeOfCharactersInGame())
            {
                OnlySurvivorsLeft();
                OnlyOneMonsterLeft();
            }
        }

        private void OnlyOneMonsterLeft()
        {
            List<User> aliveUsers = new List<User>();
            foreach (User user in Users)
            {
                if (user.Character.IsAlive)
                    aliveUsers.Add(user);
            }
            if (aliveUsers.Count == 1)
            {
                if (aliveUsers[0].Character.Type == CharacterType.MONSTER)
                {
                    Winners.Add(aliveUsers[0]);
                    Active = false;
                    throw new MonsterWinsException();
                }
            }
        }

        private void OnlySurvivorsLeft()
        {
            List<User> aliveUsers = new List<User>();
            foreach (User user in Users)
            {
                if (user.Character.IsAlive)
                    aliveUsers.Add(user);
            }
            bool allSurvivors = true;
            foreach (User user in Users)
            {
                if (user.Character.Type == CharacterType.MONSTER)
                    allSurvivors = false;
            }
            if (allSurvivors)
            {
                Winners = aliveUsers;
                Active = false;
                throw new SurvivorsWinException();
            }
        }

        private bool BothTypeOfCharactersInGame()
        {
            bool survivor = false;
            bool monster = false;
            foreach (User user in Users)
            {
                if (user.Character.Type == CharacterType.MONSTER)
                    monster = true;
                if (user.Character.Type == CharacterType.SURVIVOR)
                    survivor = true;
            }
            return monster && survivor;
        }

        private void ProcessAllTurns()
        {
            bool finishedRound = true;
            foreach (User user in Users)
            {
                if (user.Turn < 2)
                    finishedRound = false;
            }
            if (finishedRound)
            {
                foreach (User user in Users)
                {
                    user.Turn = 0;
                }
            }
        }

        private void MoveInsideBounds(Tuple<int, int> position, Direction direction)
        {
            switch (direction)
            {
                case Direction.UP:
                    if (position.Item1 == FIRST_ROW)
                        throw new MoveOutOfBoundsException();
                    break;
                case Direction.DOWN:
                    if (position.Item1 == LAST_ROW)
                        throw new MoveOutOfBoundsException();
                    break;
                case Direction.RIGHT:
                    if (position.Item2 == LAST_COL)
                        throw new MoveOutOfBoundsException();
                    break;
                case Direction.LEFT:
                    if (position.Item2 == FIRST_COL)
                        throw new MoveOutOfBoundsException();
                    break;
                default:
                    throw new InvalidMoveException();
            }
        }

        private void IsEmptySlot(User user, Tuple<int, int> position, Direction direction)
        {
            switch (direction)
            {
                case Direction.UP:
                    if (Map[position.Item1 - 1, position.Item2] != null)
                        throw new OccupiedSlotException();
                    break;
                case Direction.DOWN:
                    if (Map[position.Item1 + 1, position.Item2] != null)
                        throw new OccupiedSlotException();
                    break;
                case Direction.RIGHT:
                    if (Map[position.Item1, position.Item2 + 1] != null)
                        throw new OccupiedSlotException();
                    break;
                case Direction.LEFT:
                    if (Map[position.Item1, position.Item2 - 1] != null)
                        throw new OccupiedSlotException();
                    break;
                default:
                    throw new InvalidMoveException();
            }
        }

        private void MovePlayerTile(User user, Tuple<int, int> position, Direction direction)
        {
            if (Active)
            {
                switch (direction)
                {
                    case Direction.UP:
                        Map[position.Item1, position.Item2] = null;
                        Map[position.Item1 - 1, position.Item2] = user;
                        break;
                    case Direction.DOWN:
                        Map[position.Item1, position.Item2] = null;
                        Map[position.Item1 + 1, position.Item2] = user;
                        break;
                    case Direction.RIGHT:
                        Map[position.Item1, position.Item2] = null;
                        Map[position.Item1, position.Item2 + 1] = user;
                        break;
                    case Direction.LEFT:
                        Map[position.Item1, position.Item2] = null;
                        Map[position.Item1, position.Item2 - 1] = user;
                        break;
                    default:
                        throw new InvalidMoveException();
                }
            }
            else
                throw new EndOfMatchException();
        }

        private Tuple<int, int> FindUserPosition(User user)
        {
            Tuple<int, int> returnTuple = null;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    if (Map[i, j].Equals(user))
                        returnTuple = new Tuple<int, int>(i, j);
                }
            }
            if (returnTuple != null)
                return returnTuple;
            else
                throw new UserIsDeadException();
        }

        public string PlayerAttack(User user, Direction direction)
        {
            lock (lockMap)
            {
                Tuple<int, int> position = FindUserPosition(user);
                AttackInsideBounds(position, direction);
                AttackTarget(user, position, direction);
                return GetCloseUsersList(user);
            }
        }

        private void AttackInsideBounds(Tuple<int, int> position, Direction direction)
        {
            switch (direction)
            {
                case Direction.UP:
                    if (position.Item1 == FIRST_ROW)
                        throw new AttackOutOfBoundsException();
                    break;
                case Direction.DOWN:
                    if (position.Item1 == LAST_ROW)
                        throw new AttackOutOfBoundsException();
                    break;
                case Direction.RIGHT:
                    if (position.Item2 == LAST_COL)
                        throw new AttackOutOfBoundsException();
                    break;
                case Direction.LEFT:
                    if (position.Item2 == FIRST_COL)
                        throw new AttackOutOfBoundsException();
                    break;
                default:
                    throw new InvalidAttackException();
            }
        }

        private void AttackTarget(User user, Tuple<int, int> position, Direction direction)
        {
            switch (direction)
            {
                case Direction.UP:
                    if (Map[position.Item1 - 1, position.Item2] != null)
                    {
                        Tuple<int, int> targetPosition = new Tuple<int, int>(position.Item1 - 1, position.Item2);
                        ApplyAttack(user, targetPosition);
                    }
                    else
                    {
                        throw new EmptyAttackTargetException();
                    }
                    break;
                case Direction.DOWN:
                    if (position.Item1 == LAST_ROW)
                        throw new EmptyAttackTargetException();
                    break;
                case Direction.RIGHT:
                    if (position.Item2 == LAST_COL)
                        throw new EmptyAttackTargetException();
                    break;
                case Direction.LEFT:
                    if (position.Item2 == FIRST_COL)
                        throw new EmptyAttackTargetException();
                    break;
                default:
                    throw new InvalidAttackException();
            }
        }

        private void ApplyAttack(User user, Tuple<int, int> targetPosition)
        {
            User targetUser = GetUserByPosition(targetPosition);
            switch (user.Character.Type)
            {
                case CharacterType.MONSTER:
                    SubstractLifeFromAttackedUser(targetUser, Character.MONSTER_ATTACK);
                    break;
                case CharacterType.SURVIVOR:
                    SubstractLifeFromAttackedUser(targetUser, Character.SURVIVOR_LIFE);
                    break;
            }
        }

        private void SubstractLifeFromAttackedUser(User targetUser, int attackValue)
        {
            if (targetUser.Character.Life - attackValue > 0)
            {
                targetUser.Character.Life = targetUser.Character.Life - attackValue;
            }
            else
            {
                RemovePlayerFromMatch(targetUser);
                targetUser.Character.IsAlive = false;
            }
        }

        private void RemovePlayerFromMatch(User user)
        {
            Tuple<int, int> position = FindUserPosition(user);
            Map[position.Item1, position.Item2] = null;
            //VER SI HAY QUE NOTIFICAR A ALGUIEN
        }

        private User GetUserByPosition(Tuple<int, int> targetPosition)
        {
            return Map[targetPosition.Item1, targetPosition.Item2];
        }

        private string GetCloseUsersList(User user)
        {
            Tuple<int, int> userPosition = FindUserPosition(user);
            string closeUsers = "Coordenadas actuales: "+userPosition.Item1+","+userPosition.Item2+". Jugadores cercanos: ";
            if (userPosition.Item1 != LAST_ROW && Map[userPosition.Item1 + 1, userPosition.Item2] != null)
            {
                User closeUser = Map[userPosition.Item1 + 1, userPosition.Item2];
                if (closeUser.Character.Type == CharacterType.MONSTER)
                    closeUsers += "Monstruo, ";
                else
                    closeUsers += "Sobreviviente, ";
            }
            if (userPosition.Item1 != FIRST_ROW && Map[userPosition.Item1 - 1, userPosition.Item2] != null)
            {
                User closeUser = Map[userPosition.Item1 + 1, userPosition.Item2];
                if (closeUser.Character.Type == CharacterType.MONSTER)
                    closeUsers += "Monstruo, ";
                else
                    closeUsers += "Sobreviviente, ";
            }
            if (userPosition.Item2 != LAST_COL && Map[userPosition.Item1, userPosition.Item2 + 1] != null)
            {
                User closeUser = Map[userPosition.Item1 + 1, userPosition.Item2];
                if (closeUser.Character.Type == CharacterType.MONSTER)
                    closeUsers += "Monstruo, ";
                else
                    closeUsers += "Sobreviviente, ";
            }
            if (userPosition.Item2 != FIRST_COL && Map[userPosition.Item1, userPosition.Item2 - 1] != null)
            {
                User closeUser = Map[userPosition.Item1 + 1, userPosition.Item2];
                if (closeUser.Character.Type == CharacterType.MONSTER)
                    closeUsers += "Monstruo, ";
                else
                    closeUsers += "Sobreviviente, ";
            }
            if (userPosition.Item1 != LAST_ROW && userPosition.Item2 != LAST_COL && Map[userPosition.Item1 + 1, userPosition.Item2 + 1] != null)
            {
                User closeUser = Map[userPosition.Item1 + 1, userPosition.Item2];
                if (closeUser.Character.Type == CharacterType.MONSTER)
                    closeUsers += "Monstruo, ";
                else
                    closeUsers += "Sobreviviente, ";
            }
            if (userPosition.Item1 != FIRST_ROW && userPosition.Item2 != FIRST_COL && Map[userPosition.Item1 - 1, userPosition.Item2 - 1] != null)
            {
                User closeUser = Map[userPosition.Item1 + 1, userPosition.Item2];
                if (closeUser.Character.Type == CharacterType.MONSTER)
                    closeUsers += "Monstruo, ";
                else
                    closeUsers += "Sobreviviente, ";
            }
            if (userPosition.Item1 != FIRST_ROW && userPosition.Item2 != LAST_COL && Map[userPosition.Item1 - 1, userPosition.Item2 + 1] != null)
            {
                User closeUser = Map[userPosition.Item1 + 1, userPosition.Item2];
                if (closeUser.Character.Type == CharacterType.MONSTER)
                    closeUsers += "Monstruo, ";
                else
                    closeUsers += "Sobreviviente, ";
            }
            if (userPosition.Item1 != LAST_ROW && userPosition.Item2 != FIRST_COL && Map[userPosition.Item1 + 1, userPosition.Item2 - 1] != null)
            {
                User closeUser = Map[userPosition.Item1 + 1, userPosition.Item2];
                if (closeUser.Character.Type == CharacterType.MONSTER)
                    closeUsers += "Monstruo, ";
                else
                    closeUsers += "Sobreviviente, ";
            }
            return closeUsers;
        }
    }
}
