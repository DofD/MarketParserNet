using System;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    public abstract class CacheBase<T> : ICache<string, T>, IDisposable
    {
        /// <summary>
        ///     Генератор хеш суммы
        /// </summary>
        private readonly IHashGenerator _hashGenerator;

        protected CacheBase(IHashGenerator hashGenerator)
        {
            this._hashGenerator = hashGenerator;
        }

        /// <summary>
        ///     Возвращает элемент кеша
        /// </summary>
        /// <param name="id">Ключ</param>
        /// <returns>Элемент кеша</returns>
        public abstract T Get(string id);

        /// <summary>
        ///     Добавить элемент
        /// </summary>
        /// <param name="element">Элемент</param>
        /// <param name="ttl">Время жизни в секундах</param>
        /// <returns>Ключ</returns>
        public virtual string Add(T element, int? ttl = null)
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
        public abstract void Add(string key, T element, int? ttl = null);

        /// <summary>
        ///     Сбросить елемент кеша
        /// </summary>
        /// <param name="id">Ключ</param>
        /// <returns>Успех операции</returns>
        public abstract bool Reset(string id);

        /// <summary>
        ///     Очистить кеш
        /// </summary>
        public abstract void Clear();

        /// <summary>
        ///     Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых
        ///     ресурсов.
        /// </summary>
        public abstract void Dispose();

        protected string GetMd5(T element)
        {
            return this._hashGenerator.GetMd5(element);
        }
    }
}