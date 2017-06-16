namespace MarketParserNet.Framework.Entities
{
    using System;

    /// <summary>
    ///     Продукт
    /// </summary>
    public class Product
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
    }
}