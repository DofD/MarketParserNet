using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using MarketParserNet.Domain.Impl;
using MarketParserNet.Domain.Impl.Caches;
using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Bootstrap
{
    public class CacheInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     Инсталлировать в контейнер все необходимые элементы службы
        /// </summary>
        /// <param name="container">Контейнер</param>
        /// <param name="store">Хранилище конфигурации</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(
                Component.For(typeof(ICache<,>))
                    .ImplementedBy(typeof(CacheRedis<>))
                    .Named("SecondCache")
                    .LifestyleSingleton());

            container.Register(
                Component.For(typeof(ICache<,>))
                    .ImplementedBy(typeof(CacheMemory<>))
                    .Named("FirstCache")
                    .LifestyleSingleton());

            container.Register(
                Component.For(typeof(ICache<,>))
                    .ImplementedBy(typeof(CacheProvider<>))
                    .DependsOn(Dependency.OnComponent("cacheFirst", "FirstCache"))
                    .DependsOn(Dependency.OnComponent("cacheSecond", "SecondCache"))
                    .IsDefault()
                    .LifestyleSingleton());
        }
    }
}