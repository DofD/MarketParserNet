using System.Threading;
using System.Threading.Tasks;

using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    ///     Базовый сервис
    /// </summary>
    public abstract class BaseService : IService
    {
        protected readonly ILogger _logger;

        protected bool _disposed;

        private Task task;

        private CancellationTokenSource tokenSource;

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
                // Запускаем действие
                this.tokenSource = new CancellationTokenSource();
                var token = this.tokenSource.Token;
                this.task = Task.Run(() => this.ServiceAction(), token);
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
        ///     Действие сервиса
        /// </summary>
        protected abstract void ServiceAction();

        /// <summary>
        ///     Уничтожает экземпляр класса <see cref="ParserService" />.
        /// </summary>
        ~BaseService()
        {
            this.Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (this._disposed)
            {
                return;
            }

            if (disposing)
            {
                this.tokenSource.Dispose();
            }

            this._disposed = true;
        }
    }
}