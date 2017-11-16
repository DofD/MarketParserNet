namespace MarketParserNet.Bootstrap
{
    using System;

    using Castle.MicroKernel.Registration;
    using Castle.MicroKernel.SubSystems.Configuration;
    using Castle.Windsor;

    using Domain.Impl.DataAccess;

    using Framework.DataAccess;
    using Framework.Entities;

    public class RepositoryInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Регистрируем репозиторий продуктов
            container.Register(Component.For<IRepository<Guid, Product>>().ImplementedBy<ProductRepository>().LifestyleSingleton());
        }
    }
}
