using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    public class SpiderService : BaseService, ISpiderService
    {
        public SpiderService(ILogger logger)
            : base(logger)
        {
        }

        /// <summary>
        ///     Действие сервиса
        /// </summary>
        protected override void ServiceAction()
        {
            throw new System.NotImplementedException();
        }
    }
}