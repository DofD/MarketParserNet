namespace MarketParserNet.Framework.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq.Expressions;

    /// <summary>
    ///     Репозиторий
    /// </summary>
    /// <typeparam name="TId">Тип идентификатора</typeparam>
    /// <typeparam name="TEntity">Тип сущности</typeparam>
    public interface IRepository<in TId, TEntity>
    {
        /// <summary>
        ///     Удалить объекты
        /// </summary>
        /// <param name="ids">Идентификаторы</param>
        void Delete(params TId[] ids);

        /// <summary>
        ///     Удалить объекты
        /// </summary>
        /// <param name="entities">Удаляемые объекты</param>
        void Delete(params TEntity[] entities);

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
        ///     Сохранить объект
        /// </summary>
        /// <param name="entities">Объекты для сохранения</param>
        void Insert(params TEntity[] entities);

        /// <summary>
        ///     Сохранить или обновить
        /// </summary>
        /// <param name="entities">Объекты</param>
        void InsertOrUpdate(params TEntity[] entities);

        /// <summary>
        ///     Обновить объект
        /// </summary>
        /// <param name="entities">Обновляемый объект</param>
        void Update(params TEntity[] entities);
    }
}