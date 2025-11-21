using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ElfSoft.Framework
{
    public static class Utils
    {
        /// <summary>
        /// 如果加载句柄有效则释放
        /// </summary>
        public static void TryRelease<T>(this AsyncOperationHandle<T> handel)
        {
            if (handel.IsValid()) handel.Release();
        }

        /// <summary>
        /// 退出游戏
        /// </summary>
        public static void ExitGame()
        {
#if UNITY_EDITOR
            if (Application.isEditor)
            {
                UnityEditor.EditorApplication.isPlaying = false;
                return;
            }
#endif
            Application.Quit();
        }

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
    }
}
