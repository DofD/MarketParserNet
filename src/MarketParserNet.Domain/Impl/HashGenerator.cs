using System.Linq;
using System.Security.Cryptography;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    ///     Реализация генератора
    /// </summary>
    public class HashGenerator : IHashGenerator
    {
        private readonly ISerializer _serializer;

        public HashGenerator(ISerializer serializer)
        {
            this._serializer = serializer;
        }

        /// <summary>
        ///     Получить MD5 для объекта
        /// </summary>
        /// <param name="element">Объект</param>
        /// <returns>Хеш MD5</returns>
        public string GetMd5(object element)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            var body = this._serializer.Serialize(element);
            var hash = md5.ComputeHash(body);

            return string.Join("", hash.Select(b => b.ToString("X2")));
        }
    }
}