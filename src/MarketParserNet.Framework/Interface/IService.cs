using System;

namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    ///     Сервис
    /// </summary>
    public interface IService : IDisposable
    {
        /// <summary>
        ///     Сервис остановлен
        /// </summary>
        bool Stopped { get; }

        /// <summary>
        ///     Запустить сервис
        /// </summary>
        void Start();

        /// <summary>
        ///     Остановить сервис
        /// </summary>
        void Stop();
    }
}