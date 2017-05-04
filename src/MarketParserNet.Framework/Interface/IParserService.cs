using System;

namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    ///     Сервис парсинга
    /// </summary>
    public interface IParserService : IDisposable
    {
        /// <summary>
        ///     Запустить сервис
        /// </summary>
        void Start();

        /// <summary>
        ///     Остановить сервис
        /// </summary>
        void Stop();

        /// <summary>
        ///     Сервис остановлен
        /// </summary>
        bool Stopped { get; }
    }
}