using System.Collections.Generic;

using Newtonsoft.Json.Linq;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    /// Парсер
    /// </summary>
    public interface IParser
    {
        /// <summary>
        /// Парсить страницы начиная с
        /// </summary>
        /// <param name="webPages">Набор начальных страниц</param>
        /// <returns>Результат парсинга</returns>
        IList<JObject> ParseStartBy(params string[] webPages);

        /// <summary>
        /// Парсить только указанные страницы
        /// </summary>
        /// <param name="webPages">Набор страниц</param>
        /// <returns>Результат парсинга</returns>
        IList<JObject> ParseOnly(params string[] webPages);
    }
}