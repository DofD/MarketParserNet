using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace MarketParserNet.Utils
{
    /// <summary>
    ///     Помощник конфигурации
    /// </summary>
    public static class ConfigurationHelper
    {
        /// <summary>
        ///     Получить настройку
        /// </summary>
        /// <param name="name">Имя ключа</param>
        /// <param name="def">Значение по умолчанию</param>
        /// <returns>Значение строкой</returns>
        public static string AppSetting(string name, string def = null)
        {
            var key =
                ConfigurationManager.AppSettings.Keys.Cast<string>()
                    .FirstOrDefault(k => name.ToLowerInvariant().Equals(k.ToLowerInvariant()));

            return key == null ? def : ConfigurationManager.AppSettings[key];
        }

        /// <summary>
        ///     Получить настройку
        /// </summary>
        /// <param name="name">Имя ключа</param>
        /// <param name="def">Значение по умолчанию</param>
        /// <returns>Значение</returns>
        public static bool AppSettingBool(string name, bool def = false)
        {
            bool result;

            var setting = AppSetting(name);
            if (!bool.TryParse(setting, out result))
            {
                result = def;
            }

            return result;
        }

        /// <summary>
        ///     Получить настройку целым числом
        /// </summary>
        /// <param name="name">Имя ключа</param>
        /// <param name="def">Значение по умолчанию</param>
        /// <returns>Значение числом</returns>
        public static int AppSettingInt(string name, int def = 0)
        {
            int result;

            var setting = AppSetting(name);
            if (!int.TryParse(setting, out result))
            {
                result = def;
            }

            return result;
        }

        /// <summary>
        ///     Получить все настройки по ключу
        /// </summary>
        /// <param name="name">Имя ключа</param>
        /// <returns>Список настроек</returns>
        public static List<string> AppSettingAll(string name)
        {
            var values = ConfigurationManager.AppSettings.GetValues(name);
            return values?.ToList() ?? new List<string>();
        }

        /// <summary>
        ///     Получить набор настроек
        /// </summary>
        /// <param name="name">Наименование</param>
        /// <param name="def">Значение по умолчанию</param>
        /// <returns>Набор настроек</returns>
        public static string[] AppSettings(string name, params string[] def)
        {
            var str = AppSetting(name);

            return string.IsNullOrEmpty(str) ? def : str.Split(',');
        }
    }
}