using System;

using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    ///     Базовый сервис
    /// </summary>
    public abstract class BaseService : IService
    {
        private readonly ILogger _logger;

        protected BaseService(ILogger logger)
        {
            this._logger = logger;
        }

        /// <summary>
        ///     Сервис остановлен
        /// </summary>
        public bool Stopped { get; private set; }

        /// <summary>
        ///     Выполняет определяемые приложением задачи, связанные с удалением, высвобождением или сбросом неуправляемых
        ///     ресурсов.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
        }

        /// <summary>
        ///     Запустить сервис
        /// </summary>
        public virtual void Start()
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
        ///     Остановить сервис
        /// </summary>
        public virtual void Stop()
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
        ///     Уничтожает экземпляр класса <see cref="ParserService" />.
        /// </summary>
        ~BaseService()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}