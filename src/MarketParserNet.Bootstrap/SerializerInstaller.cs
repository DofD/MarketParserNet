using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

using MarketParserNet.Domain.Impl;
using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Bootstrap
{
    public class SerializerInstaller : IWindsorInstaller
    {
        /// <summary>
        ///     Инсталлировать в контейнер все необходимые элементы службы
        /// </summary>
        /// <param name="container">Контейнер</param>
        /// <param name="store">Хранилище конфигурации</param>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            // Регистрируем стерилизатор
            container.Register(Component.For<ISerializer>().ImplementedBy<BinarySerializer>().LifestyleSingleton());
        }
    }
}