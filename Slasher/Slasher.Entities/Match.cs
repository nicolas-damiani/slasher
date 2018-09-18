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
        private const int FIRST_ROW = 0;
        private const int LAST_ROW = 7;
        private const int FIRST_COL = 0;
        private const int LAST_COL = 7;

        public void StartMatch(List<User> users)
        {
            Users = users;
            Map = new User[8, 8];
            InitializeMap(users);
            Active = true;
            Timer = new Timer(180000);
            Timer.Start();
        }

        private void InitializeMap(List<User> users)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Map.SetValue(null, i, j);
                }
            }
            foreach (User user in users)
            {
                SetUserPosition(user);
            }
        }

        private void SetUserPosition(User user)
        {
            Random random = new Random();
            bool assigned = false;
            while (!assigned)
            {
                int row = random.Next(0, 7);
                int col = random.Next(0, 7);
                if (Map[row, col] == null)
                {
                    Map[row, col] = user;
                    assigned = true;
                }
            }
        }

        private void MovePlayer(User user, Move move)
        {
            Tuple<int, int> position = FindUserPosition(user);
            IsValidMove(user, position, move);
        }

        private void IsValidMove(User user, Tuple<int, int> position, Move move)
        {
            MoveInsideBounds(position, move);
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
                case Move.UPRIGHT:
                    if (position.Item1 == FIRST_ROW)//ver esto
                        throw new BoundsException();
                    break;
                default:
                    Console.WriteLine("hola");
                    break;
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
