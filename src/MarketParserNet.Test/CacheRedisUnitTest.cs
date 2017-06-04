using System;

using MarketParserNet.Domain.Impl;

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
            var cache = new CacheRedis<MockObject>(hashGenerator);
            var o = new MockObject();

            cache.Add(o, 10);
        }
    }
}
