using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Domain.Impl
{
    /// <summary>
    ///     Бинарный сериализатор
    /// </summary>
    public class BinarySerializer : ISerializer
    {
        /// <summary>
        ///     Десериализует массив байт в объект
        /// </summary>
        /// <typeparam name="T">Тип результирующего объекта</typeparam>
        /// <param name="bytes">Массив байт</param>
        /// <returns>Десериализованный объект</returns>
        public T Deserialize<T>(byte[] bytes)
        {
            return (T)this.Deserialize(bytes, typeof(T));
        }

        /// <summary>
        ///     Десериализует массив байт в объект
        /// </summary>
        /// <param name="bytes">Массив байт</param>
        /// <param name="type">Тип результирующего объекта</param>
        /// <returns>Десериализованный объект</returns>
        public object Deserialize(byte[] bytes, Type type)
        {
            using (var memStream = new MemoryStream())
            {
                memStream.Write(bytes, 0, bytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);

                var binaryFormatter = new BinaryFormatter();
                var obj = binaryFormatter.Deserialize(memStream);

                return obj;
            }
        }

        /// <summary>
        ///     Сериализовать объект
        /// </summary>
        /// <param name="item">Объект</param>
        /// <returns>Массив байт</returns>
        public byte[] Serialize(object item)
        {
            var binaryFormatter = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                binaryFormatter.Serialize(ms, item);
                return ms.ToArray();
            }
        }
    }
}