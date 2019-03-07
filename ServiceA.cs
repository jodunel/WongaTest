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
    public class ServiceA
    {
        static IConnection conn; // connection to RabbitMQ  service
        static public IModel channel;

        //==========================================================
        // Program entry point
        //==========================================================
        static public void Main()
        {
            ConnectMQ();
            channel = OpenNameChannel("name");
            string name;

            do
            {
                Console.Write("Enter name (Blank name to exit) : "); 
                name =  Console.ReadLine();
                if (name != "") 
                    sendNameMessage(name);
            }
            while (name != "");

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

        //==========================================================
        // send message
        //==========================================================
        public static void sendNameMessage(string name)
        {
            string message;

            message = "Hello my name is," + name;

            var body = Encoding.UTF8.GetBytes(message);
            channel.BasicPublish(exchange: "", routingKey: "name", basicProperties: null, body: body);
            Console.WriteLine(" Sent : {0}", message);

        }
    }
}

