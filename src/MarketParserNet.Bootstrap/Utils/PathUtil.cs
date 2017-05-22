using System;
using System.IO;
using System.Reflection;

namespace MarketParserNet.Bootstrap.Utils
{
    public static class PathUtil
    {
        /// <summary>
        ///     Текущий путь сборки
        /// </summary>
        public static string AssemblyDirectory
        {
            get
            {
                var assembly = Assembly.GetExecutingAssembly();

                return GetAssemblyDirectory(assembly);
            }
        }

        /// <summary>
        ///     Получить путь к сборки
        /// </summary>
        /// <param name="assembly">Сборка</param>
        /// <returns>Полный путь</returns>
        public static string GetAssemblyDirectory(Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(path);
        }
    }
}