using Castle.Core.Logging;
using Castle.Windsor;
using Castle.Windsor.Installer;

namespace MarketParserNet.Console
{
    internal class Program
    {
        private static readonly IWindsorContainer Container = new WindsorContainer();

        private static ILogger Logger;

        private static void Main(string[] args)
        {
            Bootstrap();

            Logger = Container.Resolve<ILogger>();

            Logger.Debug("Запустили парсер");

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