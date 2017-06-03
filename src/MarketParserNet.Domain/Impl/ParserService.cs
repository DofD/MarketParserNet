﻿using Castle.Core.Logging;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    ///     Сервис парсинга
    /// </summary>
    public class ParserService : BaseService, IParserService
    {
        public ParserService(ILogger logger)
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