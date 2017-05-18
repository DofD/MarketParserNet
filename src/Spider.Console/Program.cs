using System;

using Castle.Core.Logging;
using Castle.Windsor;
using Castle.Windsor.Installer;

using MarketParserNet.Framework.Interface;

namespace Spider.Console
{
    internal class Program
    {

        private static readonly IWindsorContainer Container = new WindsorContainer();

        private static ILogger Logger;

        private static void Main(string[] args)
        {
            Bootstrap();

            Logger = Container.Resolve<ILogger>();

            // Запускаем сервис
            var service = Container.Resolve<ISpiderService>();
            try
            {
                service.Start();
                Logger.Debug("Запустили сервис паука");
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