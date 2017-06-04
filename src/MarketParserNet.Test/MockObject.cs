using System;

namespace MarketParserNet.Test
{
    /// <summary>
    ///     Тестовый объект заглушка
    /// </summary>
    [Serializable]
    public class MockObject : IEquatable<MockObject>
    {
        /// <summary>
        ///     Сообщение
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        ///     Указывает, равен ли текущий объект другому объекту того же типа.
        /// </summary>
        /// <returns>
        ///     true, если текущий объект равен параметру <paramref name="other" />, в противном случае — false.
        /// </returns>
        /// <param name="other">Объект, который требуется сравнить с данным объектом.</param>
        public bool Equals(MockObject other)
        {
            return other != null && this.Message.Equals(other.Message);
        }

        /// <summary>
        ///     Определяет, равен ли заданный объект текущему объекту.
        /// </summary>
        /// <returns>
        ///     Значение true, если указанный объект равен текущему объекту; в противном случае — значение false.
        /// </returns>
        /// <param name="obj">Объект, который требуется сравнить с текущим объектом. </param>
        public override bool Equals(object obj)
        {
            return this.Equals(obj as MockObject);
        }

        /// <summary>
        /// Служит хэш-функцией по умолчанию. 
        /// </summary>
        /// <returns>
        /// Хэш-код для текущего объекта.
        /// </returns>
        public override int GetHashCode()
        {
            return this.Message?.GetHashCode() ?? 0;
        }
    }
}