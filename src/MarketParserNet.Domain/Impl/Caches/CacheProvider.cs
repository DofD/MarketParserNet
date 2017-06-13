using System;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl.Caches
{
    public class CacheProvider<T> : CacheBase<T>
        where T : class
    {
        private readonly ICache<T> _cacheFirst;

        private readonly ICache<T> _cacheSecond;

        private bool _disposed;

        public CacheProvider(IHashGenerator hashGenerator, ICache<T> cacheFirst, ICache<T> cacheSecond)
            : base(hashGenerator)
        {
            this._cacheFirst = cacheFirst;
            this._cacheSecond = cacheSecond;
        }

        /// <summary>
        ///     Возвращает элемент кеша
        /// </summary>
        /// <param name="id">Ключ</param>
        /// <returns>Элемент кеша</returns>
        public override T Get(string id)
        {
            return this._cacheFirst.Get(id) ?? this._cacheSecond.Get(id);
        }

        /// <summary>
        ///     Добавить новый элемент
        /// </summary>
        /// <param name="key">Ключ</param>
        /// <param name="element">Элемент</param>
        /// <param name="ttl">Время жизни в секундах</param>
        public override void Add(string key, T element, int? ttl = null)
        {
            this._cacheFirst.Add(key, element, ttl);
            this._cacheSecond.Add(key, element, ttl);
        }

        /// <summary>
        ///     Сбросить елемент кеша
        /// </summary>
        /// <param name="id">Ключ</param>
        /// <returns>Успех операции</returns>
        public override bool Reset(string id)
        {
            return this._cacheFirst.Reset(id) || this._cacheSecond.Reset(id);
        }

        /// <summary>
        ///     Очистить кеш
        /// </summary>
        public override void Clear()
        {
            this._cacheFirst.Clear();
            this._cacheSecond.Clear();
        }

        /// <summary>
        ///     Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых
        ///     ресурсов.
        /// </summary>
        public override void Dispose()
        {
            this.Dispose(true);
        }

        ~CacheProvider()
        {
            this.Dispose(false);
        }

        public void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                (this._cacheFirst as IDisposable)?.Dispose();
                (this._cacheSecond as IDisposable)?.Dispose();
            }

            this._disposed = true;
        }
    }
}