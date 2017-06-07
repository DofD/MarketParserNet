using System;
using System.Runtime.Caching;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    public class CacheMemory<T> : CacheBase<T>
    {
        private const double defaultTtl = 15000; // TODO вынести в конфиг

        private readonly MemoryCache _cache = new MemoryCache("CacheMemory");

        private bool _disposed;

        /// <summary>
        ///     Возвращает элемент кеша
        /// </summary>
        /// <param name="id">Ключ</param>
        /// <returns>Элемент кеша</returns>
        public override T Get(string id)
        {
            return (T)this._cache.Get(id);
        }

        /// <summary>
        ///     Добавить новый элемент
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="element">Элемент</param>
        /// <param name="ttl">Время жизни в секундах</param>
        public override void Add(string key, T element, int? ttl = null)
        {
            this._cache.Add(key, element, DateTimeOffset.Now.AddSeconds(ttl ?? defaultTtl));
        }

        /// <summary>
        ///     Сбросить елемент кеша
        /// </summary>
        /// <param name="id">Ключ</param>
        /// <returns>Успех операции</returns>
        public override bool Reset(string id)
        {
            return this._cache.Remove(id) != null;
        }

        /// <summary>
        ///     Очистить кеш
        /// </summary>
        public override void Clear()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых
        ///     ресурсов.
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(true);
        }

        ~CacheMemory()
        {
            this.Dispose(false);
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
                this._cache.Dispose();
            }

            this._disposed = true;
        }

        public CacheMemory(IHashGenerator hashGenerator)
            : base(hashGenerator)
        {
        }
    }
}