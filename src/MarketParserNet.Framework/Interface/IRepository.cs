using System.Collections.Generic;

namespace MarketParserNet.Framework.Interface
{
    /// <summary>
    ///     Репозиторий
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора</typeparam>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    public interface IRepository<in TId, TEntity>
    {
        /// <summary>
        ///     Получить объект
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Объект из БД</returns>
        TEntity Get(TId id);

        /// <summary>
        ///     Получить выборку объектов
        /// </summary>
        /// <returns>Выборка объектов</returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        ///     Сохранить или обновить
        /// </summary>
        /// <param name="entities">Объекты</param>
        void InsertOrUpdate(params TEntity[] entities);
    }
}