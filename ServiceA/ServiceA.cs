//==========================================================
// Wonga C# test program to demonstrate Micro-services using RabbitMQ
// this ServiceA console application sends a  name message to the ServiceB receiver console application.
//
// Author : Jeremy Robertson
// Date : 07 March 2019
//==========================================================
 
using System;
using RabbitMQ.Client;
using System.Text;

namespace WongaTest
{
    class ServiceA
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
            string message;
            string name;

            do
            {
                Console.Write("Enter name (Blank name to exit) : "); 
                name =  Console.ReadLine(); 
                message = messageHeader + name;

                var body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "", routingKey: "name", basicProperties: null, body: body);
                Console.WriteLine(" Sent {0}", message);
            }
            while (name != "");

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

