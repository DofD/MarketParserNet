using System;

namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    ///     Интерфейс очереди
    /// </summary>
    /// <typeparam name="T">Тип элементов в очереди</typeparam>
    /// <typeparam name="M">Тип маски (под очереди)</typeparam>
    public interface IQueue<T, in M>
    {
        /// <summary>
        ///     Вернуть актуальный элемент очереди
        /// </summary>
        /// <param name="mask">Маска</param>
        /// <returns>Элемент очереди</returns>
        T Dequeue(M mask);

        /// <summary>
        ///     Добавить новый элемент в очередь
        /// </summary>
        /// <param name="item">Добавляемый элемент</param>
        /// <param name="mask">Маска</param>
        void Enqueue(T item, M mask);

        /// <summary>
        ///     Подписаться на автоматическое получение сообщений
        /// </summary>
        /// <param name="mask">Маска</param>
        /// <param name="action">Обработчик сообщения</param>
        /// <returns>Ссылка на подписку</returns>
        IDisposable Subscribe(M mask, Func<QueueMessage, bool> action);
    }

    /// <summary>
    ///     Интерфейс очереди
    /// </summary>
    public interface IQueue : IQueue<QueueMessage, string>
    {
    }
}