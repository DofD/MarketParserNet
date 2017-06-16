namespace MarketParserNet.Framework.Entities
{
    using System;

    /// <summary>
    ///     Продукт
    /// </summary>
    public class Product : IEquatable<Product>
    {
        /// <summary>
        ///     Идентификатор
        /// </summary>
        public Guid Id { get; set; }

        /// <summary>
        ///     Наименование
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     Стоимость
        /// </summary>
        public decimal Cost { get; set; }

        /// <summary>
        ///     Страна производства
        /// </summary>
        public string MadeIn { get; set; }

        /// <summary>
        ///     Бренд
        /// </summary>
        public string Brand { get; set; }

        /// <summary>
        ///     Изображение
        /// </summary>
        public byte[] Image { get; set; }

        /// <summary>
        ///     Категория
        /// </summary>
        public string Category { get; set; }

        /// <summary>
        ///     Указывает, эквивалентен ли текущий объект другому объекту того же типа.
        /// </summary>
        /// <param name="other">Объект, который требуется сравнить с данным объектом.</param>
        /// <returns>
        ///     true, если текущий объект эквивалентен параметру <paramref name="other" />, в противном случае — false.
        /// </returns>
        public bool Equals(Product other)
        {
            if (other == null)
            {
                return false;
            }

            return Name == other.Name && Brand == other.Brand && Category == other.Category && Cost == other.Cost
                   && MadeIn == other.MadeIn;
        }

        /// <summary>
        ///     Определяет, равен ли заданный объект текущему объекту.
        /// </summary>
        /// <param name="obj">Объект, который требуется сравнить с текущим объектом.</param>
        /// <returns>
        ///     Значение true, если указанный объект равен текущему объекту; в противном случае — значение false.
        /// </returns>
        public override bool Equals(object obj)
        {
            return Equals(obj as Product);
        }

        /// <summary>
        ///     Служит хэш-функцией по умолчанию.
        /// </summary>
        /// <returns>
        ///     Хэш-код для текущего объекта.
        /// </returns>
        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Name?.GetHashCode() ?? 0;
                hashCode = (hashCode * 397) ^ Cost.GetHashCode();
                hashCode = (hashCode * 397) ^ (MadeIn?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Brand?.GetHashCode() ?? 0);
                hashCode = (hashCode * 397) ^ (Category?.GetHashCode() ?? 0);
                return hashCode;
            }
        }

        public static bool operator ==(Product p1, Product p2)
        {
            return (p1?.Equals(p2) ?? p2?.Equals(null)) ?? true;
        }

        public static bool operator !=(Product p1, Product p2)
        {
            return !(p1 == p2);
        }
    }
}