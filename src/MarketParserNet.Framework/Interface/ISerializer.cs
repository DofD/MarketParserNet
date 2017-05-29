using System;

namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    ///     Интерфейс абстракции сериализатора
    /// </summary>
    public interface ISerializer
    {
        /// <summary>
        ///     Десериализует массив байт в объект
        /// </summary>
        /// <typeparam name="T">Тип результирующего объекта</typeparam>
        /// <param name="bytes">Массив байт</param>
        /// <returns>Десериализованный объект</returns>
        T Deserialize<T>(byte[] bytes);

        /// <summary>
        ///     Десериализует массив байт в объект
        /// </summary>
        /// <param name="bytes">Массив байт</param>
        /// <param name="type">Тип результирующего объекта</param>
        /// <returns>Десериализованный объект</returns>
        object Deserialize(byte[] bytes, Type type);

        /// <summary>
        ///     Сериализовать объект
        /// </summary>
        /// <param name="item">Объект</param>
        /// <returns>Массив байт</returns>
        byte[] Serialize(object item);
    }
}