﻿using System;
using System.Threading;

using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

using StackExchange.Redis;

namespace MarketParserNet.Domain.Impl.Caches
{
    /// <summary>
    ///     Кеш на основе Redis
    /// </summary>
    /// <typeparam name="T">Тип объекта</typeparam>
    public class CacheRedis<T> : ICache<T>, IDisposable
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

        private bool _disposed;

        public CacheRedis(
            IRedisConfig config,
            IHashGenerator hashGenerator,
            ISerializer serializer,
            ILogger logger)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            if (hashGenerator == null)
            {
                throw new ArgumentNullException(nameof(hashGenerator));
            }
            if (serializer == null)
            {
                throw new ArgumentNullException(nameof(serializer));
            }
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }

            this._hashGenerator = hashGenerator;
            this._logger = logger;
            this._serializer = serializer;
            this._config = config;
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
            return this.Action(
                cache =>
                {
                    var value = cache.StringGet(id);
                    return string.IsNullOrWhiteSpace(value) ? default(T) : this._serializer.Deserialize<T>(value);
                });
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

            this.Add(key, element, ttl);
            return key;
        }

        /// <summary>
        ///     Добавить новый элемент
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="element">Элемент</param>
        /// <param name="ttl">Время жизни в секундах</param>
        public void Add(string key, T element, int? ttl = null)
        {
            this.AddOrUpdate(key, element, ttl);
        }

        /// <summary>
        ///     Сбросить элемент кеша
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Успех операции</returns>
        public bool Reset(string id)
        {
            return this.Action(cache => cache.KeyDelete(id));
        }

        /// <summary>
        ///     Очистить кеш
        /// </summary>
        public void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых
        ///     ресурсов.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     Деструктор
        /// </summary>
        ~CacheRedis()
        {
            this.Dispose(false);
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

        /// <summary>
        ///     Выполнить определяемые приложением задачи, связанные с высвобождением или сбросом неуправляемых ресурсов
        /// </summary>
        /// <param name="disposing">Освобождать</param>
        private void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this._connection?.Dispose();
            }

            this._disposed = true;
        }
    }
}