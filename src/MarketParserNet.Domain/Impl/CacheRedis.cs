using System;
using System.Threading;

using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

using StackExchange.Redis;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    ///     Кеш на основе Redis
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    public class CacheRedis<T> : ICache<string, T>
    {
        private readonly IRedisConfig _config;

        /// <summary>
        ///     Генератор хеш суммы
        /// </summary>
        private readonly IHashGenerator _hashGenerator;

        private readonly ILogger _logger;

        /// <summary>
        ///     Сериализатор
        /// </summary>
        private readonly ISerializer _serializer;

        private ConnectionMultiplexer _connection;

        public CacheRedis(
            IConfigManager<IRedisConfig> configManager,
            IHashGenerator hashGenerator,
            ISerializer serializer,
            ILogger logger)
        {
            this._hashGenerator = hashGenerator;
            this._logger = logger;
            this._serializer = serializer;
            this._config = configManager.GetConfiguration();
        }

        /// <summary>
        ///     Соединение к кешу
        /// </summary>
        private ConnectionMultiplexer Connection => this._connection ?? (this._connection = this.GetConnection());

        /// <summary>
        ///     Возвращает элемент кеша
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Элемент кеша</returns>
        public T Get(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Добавить элемент
        /// </summary>
        /// <param name="element">Элемент</param>
        /// <param name="ttl">Время жизни в секундах</param>
        /// <returns>Ключ</returns>
        public string Add(T element, int? ttl = null)
        {
            var key = this.GetMd5(element);

            this.AddOrUpdate(key, element, ttl);

            return key;
        }

        /// <summary>
        ///     Сбросить елемент кеша
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Успех операции</returns>
        public bool Reset(string id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Очистить кеш
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        private ConnectionMultiplexer GetConnection()
        {
            var i = 0;
            ConnectionMultiplexer connect = null;
            do
            {
                try
                {
                    connect = ConnectionMultiplexer.Connect(this._config.ConnectionString);

                    this._logger.Debug($"Подключились к Redis {this._config.ConnectionString}");

                    return connect;
                }
                catch (Exception exception)
                {
                    this._logger.Error($"Ошибка подключения к Redis {this._config.ConnectionString}", exception);

                    if (i < this._config.RetryCount + 1)
                    {
                        Thread.Sleep(this._config.ReconnectInterval);
                        i++;
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            while (i < this._config.RetryCount + 1);

            return connect;
        }

        /// <summary>
        ///     Операция в кеше
        /// </summary>
        /// <typeparam name="Ta">Тип результата операции</typeparam>
        /// <param name="action">Метод операции</param>
        /// <returns></returns>
        private Ta Action<Ta>(Func<IDatabase, Ta> action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }

            var cache = this.Connection.GetDatabase();

            return action(cache);
        }

        private void AddOrUpdate(string key, T element, int? ttl)
        {
            this.Action(
                cache =>
                {
                    if (cache.KeyExists(key))
                    {
                        cache.KeyDelete(key);
                    }

                    var body = this._serializer.Serialize(element);
                    if (ttl.HasValue)
                    {
                        cache.StringSet(key, body, TimeSpan.FromSeconds(ttl.Value));
                    }
                    else
                    {
                        cache.StringSet(key, body);
                    }

                    return true;
                });
        }

        private string GetMd5(T element)
        {
            return this._hashGenerator.GetMd5(element);
        }
    }
}