using System;

using Castle.Core.Logging;

using MarketParserNet.Framework;
using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    public class SpiderService : BaseService, ISpiderService
    {
        private readonly IQueue _queue;

        public SpiderService(ILogger logger, IQueue queue)
            : base(logger)
        {
            this._queue = queue;
        }       

        /// <summary>
        ///     Действие сервиса
        /// </summary>
        protected override void ServiceAction()
        {
            this._queue.Subscribe(AppCost.LinkPath, this.HandleMessage); //TODO AppCost.LinkPath в настройки
        }

        /// <summary>
        ///     Обработать сообщение
        /// </summary>
        /// <param name="message">Сообщение</param>
        /// <returns>true если обработано</returns>
        private bool HandleMessage(QueueMessage message)
        {
            throw new NotImplementedException();
        }
    }
}