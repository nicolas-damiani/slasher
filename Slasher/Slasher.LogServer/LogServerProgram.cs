using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.LogServer
{
    public class LogServerProgram
    {

        private static string SERVER_IP_ADDRESS;
        private static string queueName = @".\private$\myqueue2";//@"Formatname:DIRECT=TCP:127.0.0.1\Private$\myqueue2";

        static void Main(string[] args)
        {
            SERVER_IP_ADDRESS = "192.168.1.54";
            queueName = @"Formatname:DIRECT=TCP:"+ConfigurationManager.AppSettings["IpConfiguration"]+ @"\Private$\myqueue2";
            ShowMenu();
        }

        private static void ShowMenu()
        {
            while (true)
            {
                Console.ReadLine();
                MessageQueue msMq = new MessageQueue(queueName);
                try
                {
                    ReadLog(msMq);
                }
                catch (MessageQueueException ee)
                {
                    Console.Write(ee.ToString());
                }
                catch (Exception eee)
                {
                    Console.Write(eee.ToString());
                }
                finally
                {
                    msMq.Close();
                }
            }
        }

        private static void ReadLog(MessageQueue msMq)
        {
            msMq.Formatter = new XmlMessageFormatter(new Type[] { typeof(string) });
            Message[] msgs = msMq.GetAllMessages();
            int i = 1;
            foreach (var msg in msgs)
            {
                Console.WriteLine(i + ")" + (string)(msg.Body));
                i++;
            }
        }
    }
}
