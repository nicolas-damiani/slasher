using Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace Slasher.Entities
{
    public class Match
    {
        public List<User> Users { get; set; }
        public User[,] Map { get; set; }
        public bool Active { get; set; }
        public Timer Timer { get; set; }
        public enum Move { UP, DOWN, LEFT, RIGHT, UPRIGHT, UPLEFT, DOWNRIGHT, DOWNLEFT }
        public static Dictionary<string, Move> MovementCommands;
        private const int FIRST_ROW = 0;
        private const int LAST_ROW = 7;
        private const int FIRST_COL = 0;
        private const int LAST_COL = 7;
        private readonly object lockMap = new object();

        public Match()
        {
            MovementCommands = new Dictionary<string, Move>()
            {
                { "arriba", Move.UP},
                { "abajo", Move.DOWN},
                { "izquierda", Move.LEFT},
                { "derecha", Move.RIGHT}
            };
        }

        public void StartMatch()
        {
            Users = new List<User>();
            Map = new User[8, 8];
            InitializeMap();
            Active = true;
            Timer = new Timer(180000);
            Timer.Start();
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

        public void AddUserToMatch(User user)
        {
            SetUserRandomPosition(user);
        }

        private void SetUserRandomPosition(User user)
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
        }

        private void SetUserPosition(User user, Tuple<int, int> position)
        {
            Map[position.Item1, position.Item2] = user;
        }

        public void MovePlayer(User user, Move move)
        {
            Tuple<int, int> position = FindUserPosition(user);
            MoveInsideBounds(position, move);
            lock (lockMap)
            {
                IsEmptySlot(user, position, move);
                MovePlayerTile(user, position, move);
            }
        }

        private void MoveInsideBounds(Tuple<int, int> position, Move move)
        {
            switch (move)
            {
                case Move.UP:
                    if (position.Item1 == FIRST_ROW)
                        throw new BoundsException();
                    break;
                case Move.DOWN:
                    if (position.Item1 == LAST_ROW)
                        throw new BoundsException();
                    break;
                case Move.RIGHT:
                    if (position.Item2 == LAST_COL)
                        throw new BoundsException();
                    break;
                case Move.LEFT:
                    if (position.Item2 == FIRST_COL)
                        throw new BoundsException();
                    break;
                default:
                    throw new InvalidMoveException();
            }
        }

        private void IsEmptySlot(User user, Tuple<int, int> position, Move move)
        {
            switch (move)
            {
                case Move.UP:
                    if (Map[position.Item1 - 1, position.Item2] != null)
                        throw new OccupiedSlotException();
                    break;
                case Move.DOWN:
                    if (Map[position.Item1 + 1, position.Item2] != null)
                        throw new OccupiedSlotException();
                    break;
                case Move.RIGHT:
                    if (Map[position.Item1, position.Item2 + 1] != null)
                        throw new OccupiedSlotException();
                    break;
                case Move.LEFT:
                    if (Map[position.Item1, position.Item2 - 1] != null)
                        throw new OccupiedSlotException();
                    break;
                default:
                    throw new InvalidMoveException();
            }
        }

        private void MovePlayerTile(User user, Tuple<int, int> position, Move move)
        {
            switch (move)
            {
                case Move.UP:
                    Map[position.Item1, position.Item2] = null;
                    Map[position.Item1 - 1, position.Item2] = user;
                    break;
                case Move.DOWN:
                    Map[position.Item1, position.Item2] = null;
                    Map[position.Item1 + 1, position.Item2] = user;
                    break;
                case Move.RIGHT:
                    Map[position.Item1, position.Item2] = null;
                    Map[position.Item1, position.Item2 + 1] = user;
                    break;
                case Move.LEFT:
                    Map[position.Item1, position.Item2] = null;
                    Map[position.Item1, position.Item2 - 1] = user;
                    break;
                default:
                    throw new InvalidMoveException();
            }
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
            return returnTuple;
        }


    }
}
