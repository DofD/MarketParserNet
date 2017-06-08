using System;

using Castle.Core.Logging;

using MarketParserNet.Domain.Impl;
using MarketParserNet.Domain.Impl.Caches;
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
            using (var cache = new CacheRedis<MockObject>(new FileConfigurationRedis(), hashGenerator, new BinarySerializer(), new NullLogger()))
            {
                var o = new MockObject { Message = "AddTest" };

                cache.Add(o, 10);
            }
        }

        [TestMethod]
        public void GetTest()
        {
            var hashGenerator = new HashGenerator(new BinarySerializer());
            using (var cache = new CacheRedis<MockObject>(new FileConfigurationRedis(), hashGenerator, new BinarySerializer(), new NullLogger()))
            {
                var o = new MockObject { Message = "GetTest" };

                var key = cache.Add(o, 10);
                var cacheObject = cache.Get(key);

                Assert.AreEqual(o, cacheObject);
            }
        }

        [TestMethod]
        public void ResetTest()
        {
            var hashGenerator = new HashGenerator(new BinarySerializer());
            using (var cache = new CacheRedis<MockObject>(new FileConfigurationRedis(), hashGenerator, new BinarySerializer(), new NullLogger()))
            {
                var o = new MockObject { Message = "ResetTest" };

                var key = cache.Add(o, 10);
                cache.Reset(key);
                var cacheObject = cache.Get(key);

                Assert.IsNull(cacheObject);
            }
        }

        [TestMethod]
        public void ClearTest()
        {
            var hashGenerator = new HashGenerator(new BinarySerializer());
            using (var cache = new CacheRedis<MockObject>(new FileConfigurationRedis(), hashGenerator, new BinarySerializer(), new NullLogger()))
            {
                var o = new MockObject { Message = "ClearTest" };
                var key = cache.Add(o, 10);

                cache.Clear();
                var cacheObject = cache.Get(key);

                Assert.IsNull(cacheObject);
            }
        }
    }
}
