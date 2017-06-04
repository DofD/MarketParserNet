namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    /// Генератор хеш сумм
    /// </summary>
    public interface IHashGenerator
    {
        /// <summary>
        /// Получить MD5 для объекта
        /// </summary>
        /// <param name="element">Объект</param>
        /// <returns>Хеш MD5</returns>
        string GetMd5(object element);
    }
}