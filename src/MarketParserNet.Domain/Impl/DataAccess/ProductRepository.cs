namespace MarketParserNet.Domain.Impl.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Data.SqlClient;
    using System.Linq;

    using Dapper;

    using Framework.DataAccess;
    using Framework.Entities;

    public class ProductRepository : IRepository<Guid, Product>
    {
        private readonly string connectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        /// <summary>
        ///     Удалить объекты
        /// </summary>
        /// <param name="ids">Идентификаторы</param>
        public void Delete(params Guid[] ids)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        ///     Удалить объекты
        /// </summary>
        /// <param name="entities">Удаляемые объекты</param>
        public void Delete(params Product[] entities)
        {
            Delete(entities.Select(p => p.Id).ToArray());
        }

        /// <summary>
        ///     Получить объект
        /// </summary>
        /// <param name="id">Идентификатор</param>
        /// <returns>Объект из БД</returns>
        public Product Get(Guid id)
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Product>("SELECT * FROM Products WHERE Id = @id", new { id }).FirstOrDefault();
            }
        }

        /// <summary>
        ///     Получить выборку объектов
        /// </summary>
        /// <returns>Выборка объектов</returns>
        public IEnumerable<Product> GetAll()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                return db.Query<Product>("SELECT * FROM Products").ToList();
            }
        }

        /// <summary>
        ///     Сохранить объект
        /// </summary>
        /// <param name="entities">Объекты для сохранения</param>
        public void Insert(params Product[] entities)
        {
            const string sqlQuery = @"INSERT INTO Products
                                (Id, Name, Cost, MadeIn, Brand, Category)
                            VALUES
                                (@Id, @Name, @Cost, @MadeIn, @Brand, @Category)";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (var product in entities)
                {
                    if (product.Id == Guid.Empty)
                    {
                        product.Id = Guid.NewGuid();
                    }

                    var productId = db.Query<Guid>(sqlQuery, product).FirstOrDefault();
                }
            }
        }

        /// <summary>
        ///     Сохранить или обновить
        /// </summary>
        /// <param name="entities">Объекты</param>
        public void InsertOrUpdate(params Product[] entities)
        {
        }

        /// <summary>
        ///     Обновить объект
        /// </summary>
        /// <param name="entities">Обновляемый объект</param>
        public void Update(params Product[] entities)
        {
            const string sqlQuery = @"UPDATE Products
                                        SET Name = @Name, Cost = @Cost, MadeIn = @MadeIn, @Brand = @Brand, @Category = @Category, @Image = @Image
                                     WHERE Id = @Id";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                foreach (var product in entities)
                {
                    db.Execute(sqlQuery, product);
                }
            }
        }
    }
}