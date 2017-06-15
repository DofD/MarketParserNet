namespace MarketParserNet.Domain.Impl
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading;

    using AngleSharp.Parser.Html;

    using Castle.Core.Logging;

    using Framework.Interface;

    public class SpiderService : BaseService, ISpiderService
    {
        private readonly ICache<string> _chache;

        private readonly ISpiderServiceConfig _config;

        private readonly IQueue _queue;

        private int _curentIndex;

        public SpiderService(ISpiderServiceConfig config, IQueue queue, ICache<string> chache, ILogger logger)
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

                this._chache.Add(message.Message, message.Message);
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

        private void HandleLink(string address)
        {
            if (!address.StartsWith(this._config.StartLink))
            {
                // Обрабатываем только указаный сайт
                return;
            }

            var page = GetPage(address);
            if (string.IsNullOrEmpty(page))
            {
                return;
            }

            // Добавляем страницу в очередь парсинга
            this._queue.Enqueue(new QueueMessage { Id = Guid.NewGuid(), Message = page }, this._config.QueueParsePage);

            foreach (var href in GetHrefs(page).Where(h => h.StartsWith("http") || h.StartsWith("/") || h != this._config.StartLink))
            {
                var link = href.StartsWith("http") ? href : $"{this._config.StartLink}{href}";

                var addedKey = $"AddedQueue-{link}";
                var value = this._chache.Get(addedKey);
                if (value == null)
                {
                    _logger.Debug($"Добавили ссылку [{link}]");
                    this._queue.Enqueue(new QueueMessage { Id = Guid.NewGuid(), Message = link }, this._config.QueueLinkPath);

                    this._chache.Add(addedKey, link);
                }
            }
        }

        private static IEnumerable<string> GetHrefs(string page)
        {
            var parser = new HtmlParser();
            var document = parser.Parse(page);

            return document.QuerySelectorAll("a").Select(element => element.GetAttribute("href")).ToList();
        }

        private static string GetPage(string address)
        {
            try
            {
                var client = new WebClient();
                client.Headers.Add(
                    "user-agent",
                    "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)");

                var data = client.OpenRead(address);

                string html = null;

                if (data != null)
                {
                    using (var reader = new StreamReader(data))
                    {
                        html = reader.ReadToEnd();
                    }
                }

                return html;
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }
}