using System;

namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    /// Сообщение очереди
    /// </summary>
    [Serializable]
    public class QueueMessage
    {
        /// <summary>
        /// Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        /// Сообщение
        /// </summary>
        public string Message { get; set; }
    }
}