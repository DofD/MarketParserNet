using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using MarketParserNet.Domain.Impl.Configurations;
using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Bootstrap
{
    public class ConfigManagersInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     Инсталлировать в контейнер все необходимые элементы службы
        /// </summary>
        /// <param name="container">Контейнер</param>
        /// <param name="store">Хранилище конфигурации</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Регистрируем конфигуратор очереди
            container.Register(
               Component.For<IConfigManager<IConfigurationRabbitMq>>().ImplementedBy<ConfigManagerRabbitMQ>().LifestyleSingleton());

            // Регистрируем конфигуратор кеша
            container.Register(
               Component.For<IConfigManager<IRedisConfig>>().ImplementedBy<ConfigManagerRedis>().LifestyleSingleton());
        }
    }
}