using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
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

                this.HandleLink(message.Message);
                this._chache.Add(message.Message);
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
            const string pattern = @"href\s*=('[^']*')";
            
            if (!address.StartsWith(this._config.StartLink))
            {
                // Обрабатываем только указаный сайт
                return;
            }

            string baseUrl;
            var page = GetPage(address, out baseUrl);
            if (string.IsNullOrEmpty(page))
            {
                return;
            }

            var rgx = new Regex(pattern);

            foreach (Match match in rgx.Matches(page))
            {
                var link = Regex.Replace(match.ToString(), @"href\s*='([^']*)'", @"$1", RegexOptions.IgnoreCase);

                link = link.StartsWith("http") ? link : $"{baseUrl}{link}";
                _logger.Debug($"Получили ссылку [{link}]");

                var value = this._chache.Get(link);
                if (value == null)
                {
                    var message = new QueueMessage { Id = Guid.NewGuid(), Message = link };

                    this._queue.Enqueue(message, this._config.QueueLinkPath);
                    this._queue.Enqueue(message, "ParseLink");
                }
            }
        }

        private static string GetPage(string address, out string baseUrl)
        {
            try
            {
                var request = (HttpWebRequest)WebRequest.Create(address);
                var stream = request.GetResponse().GetResponseStream();
                string html = null;
                using (stream)
                {
                    // Указали, что из потока нужно читать в кодировке windows-1251
                    if (stream != null)
                        using (var reader = new StreamReader(stream, Encoding.GetEncoding(1251)))
                        {
                            html = reader.ReadToEnd();
                        }
                }

                baseUrl = request.Address.Scheme + "://" + request.Address.Authority + "/";
                return html;
            }
            catch (Exception exception)
            {
                baseUrl = null;
                return null;
            }
        }
    }
}