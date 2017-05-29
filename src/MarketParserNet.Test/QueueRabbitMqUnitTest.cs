using System;

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
            var queue = new QueueRabbitMq(new ConnectionFactory(), new ConfigManagerRabbitMQ(), new BinarySerializer(),  new NullLogger());
            queue.Enqueue(new QueueMessage { Id = Guid.NewGuid(), Message = "Тестовое сообщение" }, "Test");
        }

        [TestMethod]
        public void DequeueTest()
        {
            var queue = new QueueRabbitMq(new ConnectionFactory(), new ConfigManagerRabbitMQ(), new BinarySerializer(), new NullLogger());
            queue.Dequeue("Test");
        }
    }
}

