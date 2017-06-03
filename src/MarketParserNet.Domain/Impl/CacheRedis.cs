using System;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    ///     Кеш на основе Redis
    /// </summary>
    /// <typeparam name="I">Тип ключа</typeparam>
    /// <typeparam name="T">Тип объекта</typeparam>
    public class CacheRedis<I, T> : ICache<I, T>
    {
        /// <summary>
        ///     Возвращает элемент кеша
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Элемент кеша</returns>
        public T Get(I id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Добавить элемент
        /// </summary>
        /// <param name="element">Элемент</param>
        /// <returns>Ключ</returns>
        public I Add(T element)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Сбросить елемент кеша
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Успех операции</returns>
        public bool Reset(I id)
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
    }
}