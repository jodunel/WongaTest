//==========================================================
// Wonga C# test program to demonstrate Micro-services using RabbitMQ
// this ServiceB console application receives name messages from the ServiceA console application.
//
// Author : Jeremy Robertson
// Date : 07 March 2019
//==========================================================

using System;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace WongaTest
{
    public class ServiceB
    {
        private static IConnection conn; // connection to RabbitMQ  service
        private const string messageHeader = "Hello my name is,";
        public static string messageReceived = "";
        private static IModel channel;

        //==========================================================
        // Program entry point
        //==========================================================
        static public void Main()
        {
            ConnectMQ();
            channel = OpenNameChannel("name");
            var consumer = new EventingBasicConsumer(channel);

            Console.WriteLine("..Waiting for name message - Press [enter] to exit.");
            //add event handler
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                messageReceived = Encoding.UTF8.GetString(body);
                if (messageReceived.Substring(0, messageHeader.Length) == "Hello my name is,")
                    Console.WriteLine(" Hello {0}, I am your father (press [enter] to exit)", messageReceived.Substring(messageHeader.Length, messageReceived.Length - messageHeader.Length));
            };
            channel.BasicConsume(queue: "name",
                                 autoAck: true,
                                 consumer: consumer);

            //if enter pressed exit program
            Console.ReadLine();

            //close the channel and connection to RabbitMQ
            channel.Close();
            conn.Close();

        }

        //==========================================================
        //Connect to the RabbitMQ service on the localhost
        //==========================================================
        public static void ConnectMQ()
        {
            ConnectionFactory factory;

            try
            {
                factory = new ConnectionFactory() { HostName = "localhost" };
                conn = factory.CreateConnection();
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException e)
            {
                Console.WriteLine(e.Message);
            }
        }

        //==========================================================
        //Open a channel
        //==========================================================
        public static IModel OpenNameChannel(string name)
        {
            channel = null;

            try
            {
                channel = conn.CreateModel();
                channel.QueueDeclare(queue: name,
                     durable: false,
                     exclusive: false,
                     autoDelete: false,
                     arguments: null);
            }
            catch (RabbitMQ.Client.Exceptions.RabbitMQClientException e)
            {
                Console.WriteLine(e.Message);
            }

            return channel;
        }

    }
}
