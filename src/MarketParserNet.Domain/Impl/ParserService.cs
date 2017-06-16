using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    using System;
    using System.Linq;

    using Castle.Core.Internal;

    using Framework.DataAccess;
    using Framework.Entities;

    /// <summary>
    ///     Сервис парсинга
    /// </summary>
    public class ParserService : BaseService, IParserService
    {
        private readonly IParserServiceConfig _config;

        private readonly IQueue _queue;

        private readonly IParser[] _parsers;

        private readonly IRepository<Guid, Product> _repository;

        public ParserService(IParserServiceConfig config, IQueue queue, IParser[] parsers, IRepository<Guid, Product> repository, ILogger logger)
            : base(logger)
        {
            if (repository != null)
            {
                _repository = repository;
            }
            if (config != null)
            {
                _config = config;
            }
            if (queue != null)
            {
                _queue = queue;
            }
            if (parsers != null)
            {
                _parsers = parsers;
            }
        }

        /// <summary>
        ///     Действие сервиса
        /// </summary>
        protected override void ServiceAction()
        {
            this._queue.Subscribe(this._config.QueueParsePage, this.HandleMessage);
        }

        private bool HandleMessage(QueueMessage arg)
        {
            var page = arg.Message;
            var products =_parsers.SelectMany(p => p.ParseOnly(page)).Select(o => o.ToObject<Product>());

            _repository.InsertOrUpdate(products.ToArray());
            return true;
        }
    }
}