using System;

using Castle.Core.Logging;

using MarketParserNet.Domain.Impl;

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
            var queue = new QueueRabbitMq(new ConnectionFactory(), new ConfigManagerRabbitMQ(), new NullLogger());
        }
    }
}

