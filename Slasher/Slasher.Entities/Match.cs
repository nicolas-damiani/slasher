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
        public enum Direction { UP, DOWN, LEFT, RIGHT }
        public static Dictionary<string, Direction> MovementCommands;
        private const int FIRST_ROW = 0;
        private const int LAST_ROW = 7;
        private const int FIRST_COL = 0;
        private const int LAST_COL = 7;
        private readonly object lockMap = new object();

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

        public void MovePlayer(User user, Direction direction)
        {
            Tuple<int, int> position = FindUserPosition(user);
            MoveInsideBounds(position, direction);
            lock (lockMap)
            {
                IsEmptySlot(user, position, direction);
                MovePlayerTile(user, position, direction);
            }
        }

        private void MoveInsideBounds(Tuple<int, int> position, Direction direction)
        {
            switch (direction)
            {
                case Direction.UP:
                    if (position.Item1 == FIRST_ROW)
                        throw new BoundsException();
                    break;
                case Direction.DOWN:
                    if (position.Item1 == LAST_ROW)
                        throw new BoundsException();
                    break;
                case Direction.RIGHT:
                    if (position.Item2 == LAST_COL)
                        throw new BoundsException();
                    break;
                case Direction.LEFT:
                    if (position.Item2 == FIRST_COL)
                        throw new BoundsException();
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

        public void PlayerAttack(User user, Direction direction)
        {
            
        }


    }
}
