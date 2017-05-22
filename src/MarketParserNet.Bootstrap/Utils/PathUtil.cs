using System;
using System.IO;
using System.Reflection;

namespace MarketParserNet.Bootstrap.Utils
{
    public static class PathUtil
    {
        /// <summary>
        ///     ������� ���� ������
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
        ///     �������� ���� � ������
        /// </summary>
        /// <param name="assembly">������</param>
        /// <returns>������ ����</returns>
        public static string GetAssemblyDirectory(Assembly assembly)
        {
            var codeBase = assembly.CodeBase;
            var uri = new UriBuilder(codeBase);
            var path = Uri.UnescapeDataString(uri.Path);

            return Path.GetDirectoryName(path);
        }
    }
}