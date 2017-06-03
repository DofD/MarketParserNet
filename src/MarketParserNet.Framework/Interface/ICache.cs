﻿namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    ///     Интерфейс кеша
    /// </summary>
    /// <typeparam name="I">Ключ</typeparam>
    /// <typeparam name="T">Тип кеша</typeparam>
    public interface ICache<I, T>
    {
        /// <summary>
        ///     Возвращает элемент кеша
        /// </summary>
        /// <param name="id">Ключ</param>
        /// <returns>Элемент кеша</returns>
        T Get(I id);

        /// <summary>
        ///     Добавить элемент
        /// </summary>
        /// <param name="element">Элемент</param>
        /// <returns>Ключ</returns>
        I Add(T element);

        /// <summary>
        ///     Сбросить елемент кеша
        /// </summary>
        /// <param name="id">Ключ</param>
        /// <returns>Успех операции</returns>
        bool Reset(I id);

        /// <summary>
        ///     Очистить кеш
        /// </summary>
        void Clear();
    }
}