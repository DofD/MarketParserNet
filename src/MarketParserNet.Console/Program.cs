using System;

using MarketParserNet.Framework.Interface;

namespace MarketParserNet.Console
{
    using Castle.Core.Logging;
    using Castle.MicroKernel.Resolvers.SpecializedResolvers;
    using Castle.Windsor;
    using Castle.Windsor.Installer;

    internal class Program
    {
        private static readonly IWindsorContainer Container = new WindsorContainer();

        private static ILogger Logger;

        static Program()
        {
            Container.Kernel.Resolver.AddSubResolver(new ArrayResolver(Container.Kernel));
        }

        private static void Main(string[] args)
        {
            Bootstrap();

            Logger = Container.Resolve<ILogger>();

            // Запускаем сервис
            var service = Container.Resolve<IParserService>();
            try
            {
                service.Start();
                Logger.Debug("Запустили сервис");
                Logger.Debug("Для выхода нажмите любую клавишу");

                System.Console.ReadKey();
            }
            catch (Exception e)
            {
                // Выводим ошибку 
                Logger.Error("Ошибка", e);
            }
            finally
            {
                service.Stop();
                Logger.Debug("Остановили сервис");
            }

            Logger.Debug("Для выхода нажмите любую клавишу");
            System.Console.ReadKey();
        }

        /// <summary>
        ///     Начальная загрузка
        /// </summary>
        private static void Bootstrap()
        {
            Container.Install(FromAssembly.This(), FromAssembly.Named("MarketParserNet.Bootstrap"));
        }
    }
}