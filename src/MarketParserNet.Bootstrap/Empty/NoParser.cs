namespace MarketParserNet.Bootstrap.Empty
{
    using System.Collections.Generic;

    using Domain.Impl;

    using Newtonsoft.Json.Linq;

    public class NoParser : IParser
    {
        /// <summary>
        ///     Имя сайта
        /// </summary>
        public string SiteName => "Empty";

        /// <summary>
        ///     Парсить страницы начиная с
        /// </summary>
        /// <param name="webPages">Набор начальных страниц</param>
        /// <returns>Результат парсинга</returns>
        public IList<JObject> ParseStartBy(params string[] webPages)
        {
            return new List<JObject>();
        }

        /// <summary>
        ///     Парсить только указанные страницы
        /// </summary>
        /// <param name="webPages">Набор страниц</param>
        /// <returns>Результат парсинга</returns>
        public IList<JObject> ParseOnly(params string[] webPages)
        {
            return new List<JObject>();
        }
    }
}