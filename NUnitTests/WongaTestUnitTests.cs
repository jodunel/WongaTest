using NUnit.Framework;
using RabbitMQ.Client;
using WongaTest;
using RabbitMQ.Client.Events;
using System.Text;

namespace Tests
{
    public class Tests
    {
        IModel channelA;
        IModel channelB;

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void MessageTest()
        {
            ServiceA.ConnectMQ();
            channelA = ServiceA.OpenNameChannel(name: "test");
            ServiceB.ConnectMQ();
            channelB = ServiceB.OpenNameChannel(name: "test");

            ServiceA.sendNameMessage("Jeremy");

            var consumer = new EventingBasicConsumer(channelB);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body;
                ServiceB.messageReceived = Encoding.UTF8.GetString(body);
                Assert.AreEqual("Hello my name is,Jeremy", ServiceB.messageReceived, "Message failed");
            };

            channelB.BasicConsume(queue: "name",
                                 autoAck: true,
                                 consumer: consumer);
        }
    }
}