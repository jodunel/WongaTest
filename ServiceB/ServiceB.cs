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
    class ServiceB
    {
        static IConnection conn; // connection to RabbitMQ  service
        private const string messageHeader = "Hello my name is,";

        //==========================================================
        // Program entry point
        //==========================================================
        static void Main()
        {
            IModel channel;

            ConnectMQ();
            channel = OpenNameChannel("name");
            var consumer = new EventingBasicConsumer(channel);

            Console.WriteLine("..Waiting for name message - Press [enter] to exit.");
            //add event handler
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                var message = Encoding.UTF8.GetString(body);
                if (message.Substring(0, messageHeader.Length) == "Hello my name is,")
                    Console.WriteLine(" Hello {0}, I am your father (press [enter] to exit)", message.Substring(messageHeader.Length, message.Length - messageHeader.Length));
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
        private static void ConnectMQ()
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
        private static IModel OpenNameChannel(string name)
        {
            IModel channel = null;

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
