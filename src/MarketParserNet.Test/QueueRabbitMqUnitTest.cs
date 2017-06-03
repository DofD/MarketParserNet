using System;
using System.Threading;

using Castle.Core.Logging;

using MarketParserNet.Domain.Impl;
using MarketParserNet.Framework.Interface;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using RabbitMQ.Client;

namespace MarketParserNet.Test
{
    [TestClass]
    public class QueueRabbitMqUnitTest
    {
        [TestMethod]
        public void EnqueueTest()
        {
            var queue = new QueueRabbitMq(new ConnectionFactory(), new ConfigManagerRabbitMQ(), new BinarySerializer(), new NullLogger());
            queue.Enqueue(new QueueMessage { Id = Guid.NewGuid(), Message = "Тестовое сообщение" }, "Test");
        }

        [TestMethod]
        public void DequeueTest()
        {
            var queue = new QueueRabbitMq(new ConnectionFactory(), new ConfigManagerRabbitMQ(), new BinarySerializer(), new NullLogger());
            queue.Dequeue("Test");
        }

        [TestMethod]
        public void SubscribeTest()
        {
            const int count = 10;
            var taskItem = new object();

            var queue = new QueueRabbitMq(new ConnectionFactory(), new ConfigManagerRabbitMQ(), new BinarySerializer(), new NullLogger());

            var index = 0;
            var stop = false;
            queue.Subscribe("TestSubscribe", message =>
            {
                index++;

                if (index == count)
                    stop = true;

                return true;
            });

            for (var i = 0; i < count; i++)
            {
                queue.Enqueue(new QueueMessage { Id = Guid.NewGuid(), Message = $"Тестовое сообщение №{i}" }, "TestSubscribe");
            }

            while (!stop)
            {
                
            }

            Assert.AreEqual(count, index);
        }
    }
}

