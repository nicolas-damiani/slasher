﻿using Exceptions;
using Slasher.Entities;
using Slasher.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using AdminServerProgram.ServiceReference1;
using WCFHosting;

namespace AdminServerProgram
{
    class AdminServer
    {
        public static Service remoteSystem;
        static void Main(string[] args)
        {
            
            
            remoteSystem = new Service();
            bool running = true;
            while (running)
            {
                try
                {
                    ShowMenu();
                    int opc = Int32.Parse(Console.ReadLine());
                    if (opc == 1)
                    {
                        AddUserToSystem();
                    }
                    if (opc == 2)
                    {
                        ModifyUserInSystem();
                    }
                    if (opc == 3)
                    {
                        DeleteUserInSystem();
                    }
                    if (opc == 4)
                    {
                        GetUserStatistics();
                    }
                    if (opc == 5)
                    {
                        GetHighScores();
                    }
                    if (opc == 6)
                    {
                        running = false;
                    }
                }
                catch (ServerSystemException ex)
                {
                    Console.WriteLine(ex.Message);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }
        }

        private static void GetHighScores()
        {
            List<UserScore> statistics = remoteSystem.GetHighScores();
            int count = 1;
            foreach (UserScore userScore in statistics)
            {
                string character = "Monstruo";
                if (userScore.CharacterType == CharacterType.SURVIVOR)
                    character = "Sobreviviente";
                Console.WriteLine(count + ")  " + userScore.user.NickName + ", Rol: " + character + ", Fecha: " + userScore.Date + ", Puntaje: " + userScore.Score);
                count++;
            }
        }

        public static void ShowMenu()
        {
            Console.WriteLine("1) Agregar usuario");
            Console.WriteLine("2) Modificar usuario");
            Console.WriteLine("3) Eliminar usuario");
            Console.WriteLine("4) Ver estadisticas de jugadores");
            Console.WriteLine("5) Ver Highscores");
            Console.WriteLine("6) Desconectarse");
        }


        public static void AddUserToSystem()
        {
            Console.WriteLine("Ingrese nombre de usuario");
            User user = new User(Console.ReadLine());
            remoteSystem.AddUserToSystem(user);
            Console.WriteLine("Usuario ingresado correctamente.");
        }

        public static void GetUserStatistics()
        {
            List<MatchPlayerStatistic> statistics = remoteSystem.GetUserStatistics();
            foreach (MatchPlayerStatistic statistic in statistics)
            {
                Console.WriteLine("----------------------");
                Console.WriteLine("Partida " + statistic.MatchId);
                foreach (Tuple<User, bool> userInfo in statistic.userList)
                {
                    string character = "Monstruo";
                    if (userInfo.Item1.Character.Type == CharacterType.SURVIVOR)
                        character = "Sobreviviente";
                    string won = "";
                    if (userInfo.Item2)
                        won = ",   Ganador";

                    Console.WriteLine("     Jugador: " + userInfo.Item1.NickName +
                        ",   Rol: " +character +won);
                }
            }
        }

        public static void DeleteUserInSystem()
        {
            Console.WriteLine("Ingrese nombre de usuario");
            remoteSystem.DeleteUser(Console.ReadLine());
            Console.WriteLine("Usuario eliminado correctamente.");
        }

        public static void ModifyUserInSystem()
        {
            Console.WriteLine("Ingrese nombre de usuario para cambiar");
            string oldName = Console.ReadLine();
            Console.WriteLine("Ingrese nuevo nombre");
            remoteSystem.ModifyUser(oldName, Console.ReadLine());
            Console.WriteLine("Usuario modificado correctamente.");
        }
    }
}
