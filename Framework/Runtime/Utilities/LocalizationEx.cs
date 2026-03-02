using UnityEngine.Localization.Settings;

namespace ElfSoft.Framework
{
    public static class LocalizationEx
    {
        /// <summary>
        /// 获取本地化文本
        /// </summary>
        public static string GetLocalizedString(string tableName, string entryKey)
        {
            return LocalizationSettings.StringDatabase.GetLocalizedString(tableName, entryKey);
        }

        /// <summary>
        /// 将单行LocalText分割为tableName和entryKey
        /// </summary>
        public static void SplitLocalText(string text, out string tableName, out string entryKey)
        {
            int i = string.IsNullOrEmpty(text) ? -1 : text.IndexOf('/');
            tableName = i > 0 ? text[..i] : null;
            entryKey = i > 0 ? text[(i + 1)..] : null;
        }

        public static string GetLocalizedString(string tableEntry)
        {
            SplitLocalText(tableEntry, out var tableName, out var entryKey);
            return GetLocalizedString(tableName, entryKey);
        }
    }
}
