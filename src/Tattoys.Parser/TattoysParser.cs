using System;
using System.Collections.Generic;

using MarketParserNet.Domain.Impl;

using Newtonsoft.Json.Linq;

namespace Tattoys.Parser
{
    public class TattoysParser : IParser
    {
        /// <summary>
        ///     Имя сайта
        /// </summary>
        public string SiteName => "tattoys.ru";

        /// <summary>
        ///     Парсить страницы начиная с
        /// </summary>
        /// <param name="webPages">Набор начальных страниц</param>
        /// <returns>Результат парсинга</returns>
        public IList<JObject> ParseStartBy(params string[] webPages)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Парсить только указанные страницы
        /// </summary>
        /// <param name="webPages">Набор страниц</param>
        /// <returns>Результат парсинга</returns>
        public IList<JObject> ParseOnly(params string[] webPages)
        {
            throw new NotImplementedException();
        }
    }
}