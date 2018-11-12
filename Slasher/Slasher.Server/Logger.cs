using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace Slasher.Server
{
    public class Logger
    {
        public void LogData(string message)
        {
            string queueName = @".\private$\myqueue2";
            MessageQueue msMq = null;
            if (!MessageQueue.Exists(queueName))
            {
                msMq = MessageQueue.Create(queueName);
            }
            using (msMq = new MessageQueue(queueName))
            {
                try
                {
                    msMq.Send(message + " FECHA: " + DateTime.Now.ToString());
                }
                catch (MessageQueueException ee)
                {
                    Console.Write(ee.ToString());
                }
                catch (Exception eee)
                {
                    Console.Write(eee.ToString());
                }
            }
        }

        public void RemoveLogs()
        {
            string queueName = @".\private$\myqueue2";
            MessageQueue msMq = null;
            if (MessageQueue.Exists(queueName))
            {
                using (msMq = new MessageQueue(queueName))
                {
                    try
                    {
                        msMq.Purge();
                    }
                    catch (MessageQueueException ee)
                    {
                        Console.Write(ee.ToString());
                    }
                    catch (Exception eee)
                    {
                        Console.Write(eee.ToString());
                    }
                }
            }
        }
    }
}
