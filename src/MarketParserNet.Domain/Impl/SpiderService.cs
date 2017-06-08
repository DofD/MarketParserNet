using System;
using System.Threading;

using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    public class SpiderService : BaseService, ISpiderService
    {
        private readonly ICache<string, string> _chache;

        private readonly ISpiderServiceConfig _config;

        private readonly IQueue _queue;

        private int _curentIndex;

        public SpiderService(
            ISpiderServiceConfig config,
            IQueue queue,
            ICache<string, string> chache,
            ILogger logger)
            : base(logger)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }

            if (queue == null)
            {
                throw new ArgumentNullException(nameof(queue));
            }
            if (chache == null)
            {
                throw new ArgumentNullException(nameof(chache));
            }

            this._queue = queue;
            this._chache = chache;
            this._config = config;
        }

        /// <summary>
        ///     Запустить сервис
        /// </summary>
        public override void Start()
        {
            base.Start();
            var lastWorkLink = this._chache.Get("LastLink") ?? this._config.StartLink;
            if (!string.IsNullOrWhiteSpace(lastWorkLink))
            {
                this._queue.Enqueue(new QueueMessage { Message = lastWorkLink }, this._config.QueueLinkPath);
            }
        }

        /// <summary>
        ///     Действие сервиса
        /// </summary>
        protected override void ServiceAction()
        {
            this._queue.Subscribe(this._config.QueueLinkPath, this.HandleMessage);
        }

        /// <summary>
        ///     Обработать сообщение
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns>true если обработано</returns>
        private bool HandleMessage(QueueMessage message)
        {
            // Пробуем получить из кеша
            var value = this._chache.Get(message.Message);

            if (value == null)
            {
                this._logger.Debug($"Обрабатываем ссылку [{message.Message}]");

                this.HandleLink(message.Message);
            }
            else
            {
                this._logger.Debug($"Ссылка[{message.Message}] была ранее обработана");
            }

            var localIndex = Interlocked.CompareExchange(
                ref this._curentIndex,
                this._curentIndex,
                this._config.RecordingIntervals);

            if (localIndex == this._config.RecordingIntervals)
            {
                Interlocked.Increment(ref this._curentIndex);
                this._chache.Add("LastLink", message.Message);
            }

            return true;
        }

        private void HandleLink(string message)
        {
            //TODO Поиск ссылок
        }
    }
}