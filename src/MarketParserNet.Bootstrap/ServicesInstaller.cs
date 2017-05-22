using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using MarketParserNet.Bootstrap.Utils;
using MarketParserNet.Domain.Impl;
using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Bootstrap
{
    public class ServicesInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     Инсталлировать в контейнер все необходимые элементы службы
        /// </summary>
        /// <param name="container">Контейнер</param>
        /// <param name="store">Хранилище конфигурации</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Регистрируем сервис
            container.Register(
                Component.For<IParserService>().ImplementedBy<ParserService>().LifestyleSingleton());

            // Регистрируем парсеры
            container.Register(
                Classes.FromAssemblyInDirectory(new AssemblyFilter(PathUtil.AssemblyDirectory))
                    .BasedOn<IParser>()
                    .LifestyleTransient());

            // Регистрируем сервис паука
            container.Register(
                Component.For<ISpiderService>().ImplementedBy<SpiderService>().LifestyleSingleton());

            // Регистрируем очередь
            container.Register(Component.For<IQueue>().ImplementedBy<QueueRabbitMq>().LifestyleSingleton());
        }
    }
}
