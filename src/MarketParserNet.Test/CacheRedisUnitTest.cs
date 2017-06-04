using System;

using Castle.Core.Logging;

using MarketParserNet.Domain.Impl;
using MarketParserNet.Domain.Impl.Configurations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MarketParserNet.Test
{
    [TestClass]
    public class CacheRedisUnitTest
    {
        [TestMethod]
        public void AddTest()
        {
            var hashGenerator = new HashGenerator(new BinarySerializer());
            var cache = new CacheRedis<MockObject>(new ConfigManagerRedis(), hashGenerator, new BinarySerializer(), new NullLogger());
            var o = new MockObject();

            cache.Add(o, 10);
        }
    }
}
