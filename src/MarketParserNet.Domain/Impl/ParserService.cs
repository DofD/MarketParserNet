using System;

using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    ///     Сервис парсинга
    /// </summary>
    public class ParserService : IParserService
    {
        private readonly ILogger _logger;

        public ParserService(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        ///     Запустить сервис
        /// </summary>
        public void Start()
        {
            this._logger.Debug(" --> Start");
            if (this.Stopped)
            {
                // Подписываемся на получение сообщений
                this.Stopped = false;
            }
            this._logger.Debug(" <-- Start");
        }

        /// <summary>
        ///     Сервис остановлен
        /// </summary>
        public bool Stopped { get; private set; }

        /// <summary>
        ///     Остановить сервис
        /// </summary>
        public void Stop()
        {
            this._logger.Debug(" --> Stop");
            if (this.Stopped)
            {
                this._logger.Debug(" <-- Stop Уже остановлен");
                return;
            }

            this.Dispose(true);

            this._logger.Debug(" <-- Stop");
        }

        /// <summary>
        /// Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых ресурсов.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     Уничтожает экземпляр класса <see cref="ParserService" />.
        /// </summary>
        ~ParserService()
        {
            this.Dispose(false);
        }

        private void Dispose(bool disposing)
        {
        }
    }
}