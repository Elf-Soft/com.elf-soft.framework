using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.Localization.UI;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;
using LES = UnityEditor.Localization.LocalizationEditorSettings;

namespace ElfSoft.Framework.Editor
{
    public static class LocalizationEditorEx
    {
        //UnityEditor.Localization.UI.LocalizedReferencePropertyDrawer.Styles
        public static readonly string NoLocale = $"None (Locale)";
        public static readonly string NoTableEntrySelected = $"None (String)";
        //UnityEditor.Localization.EditorIcons
        public static readonly Texture2D TableWindow = EditorGUIUtility.FindTexture("Packages/com.unity.localization/Editor/Icons/" + "Localization Tables Window/LocalizationTablesWindow.png");
        //UnityEditor.Localization.Search
        public const string AssetTableProvider = "at";
        public const string AssetTableProviderFilter = AssetTableProvider + ":";
        public const string StringTableProvider = "st";
        public const string StringTableProviderFilter = StringTableProvider + ":";


        #region 获取数据
        /// <summary>
        /// 尝试获取本地化文本条目
        /// </summary>
        public static bool TryGetStringTableEntry(string tableName, string entryKey, LocaleIdentifier localIdentifier,
            out (StringTableCollection col, StringTable table, StringTableEntry entry) result)
        {
            var col = !string.IsNullOrEmpty(tableName) ? LES.GetStringTableCollection(tableName) : null;
            var table = col != null ? col.GetTable(localIdentifier) as StringTable : null;
            var entry = table != null && !string.IsNullOrEmpty(entryKey) ? table.GetEntry(entryKey) : null;
            result = new(col, table, entry);
            return entry != null;
        }

        /// <summary>
        /// 获取当前选择或默认地区
        /// </summary>
        public static bool TryGetSelectedOrDefaultLocale(out Locale locale)
        {
            locale = LES.ActiveLocalizationSettings.GetSelectedLocale();
            if (locale != null) return true;

            var ls = LES.GetLocales();
            locale = ls.Count > 0 ? ls[0] : null;
            return ls.Count > 0;
        }

        /// <summary>
        /// 获取本地化文本条目值(本地化后的)
        /// </summary>
        public static string GetStringTableEntryLocalizedValue(string tableName, string entryName)
        {
            if (!TryGetSelectedOrDefaultLocale(out var locale)) return NoLocale;

            var b = TryGetStringTableEntry(tableName, entryName, locale.Identifier, out var result);
            return b ? result.entry.LocalizedValue : null;
        }

        #endregion

        #region 编辑器窗口
        /// <summary>
        /// 打开本地化数据编辑器
        /// </summary>
        public static void ShowTableWindow(string tableName)
        {
            var col = LES.GetStringTableCollection(tableName);
            LocalizationTablesWindow.ShowWindow(col);
        }

        public static void ShowEntryWindow(string tableName, string entryKey)
        {
            var table = LES.GetStringTableCollection(tableName);
            var entry = table != null ? table.SharedData.GetEntry(entryKey) : null;
            if (entry != null) LocalizationTablesWindow.ShowWindow(tableName, entry.Key);
            else ShowTableWindow(tableName);
        }

        public static void SceneControlsWindow()
        {
            var a = typeof(LES).Assembly;
            var type = a.GetType("UnityEditor.Localization.UI.SceneControlsWindow");
            var method = type.GetMethod("ShowWindow", BindingFlags.NonPublic | BindingFlags.Static);
            method.Invoke(null, null);
        }

        public static SearchProvider GetStringTableSearchProvider()
        {
            var assembly = typeof(UnityEditor.Localization.Search.TableEntrySearchData).Assembly;
            var type = assembly.GetType("UnityEditor.Localization.Search.StringTableSearchProvider");
            return Activator.CreateInstance(type) as SearchProvider;
        }

        #endregion


        //同步列表Key
        public static void UpdateStringTableKeysById(string tableName, IEnumerable<string> keys)
        {
            var sharedData = LES.GetStringTableCollection(tableName).SharedData;
            foreach (var key in keys)
            {
                if (!sharedData.Contains(key)) sharedData.AddKey(key);
            }
            for (var i = sharedData.Entries.Count - 1; i >= 0; i--)
            {
                var k = sharedData.Entries[i].Key;
                if (!keys.Contains(k)) sharedData.RemoveKey(k);
            }
            sharedData.Entries.Sort((x, y) =>
            {
                int.TryParse(x.Key, out var xId);
                int.TryParse(y.Key, out var yId);
                return xId.CompareTo(yId);
            });
            EditorUtility.SetDirty(sharedData);
            AssetDatabase.SaveAssetIfDirty(sharedData);
        }

        /// <summary>
        /// VirualElement注册Localization编辑器事件
        /// </summary>
        public static void RegisterEditorEntryChangedEvent(VisualElement elem,
            Action<SharedTableData.SharedTableEntry> onEntryModified,
            Action<LocalizationTableCollection, SharedTableData.SharedTableEntry> onEntryRemoved)
        {
            elem.RegisterCallback<AttachToPanelEvent>(RegisterEvent);
            elem.RegisterCallback<DetachFromPanelEvent>(UnregiserEvent);

            void RegisterEvent(AttachToPanelEvent evt)
            {
                LES.EditorEvents.TableEntryModified += onEntryModified;
                LES.EditorEvents.TableEntryRemoved += onEntryRemoved;
            }
            void UnregiserEvent(DetachFromPanelEvent evt)
            {
                LES.EditorEvents.TableEntryModified -= onEntryModified;
                LES.EditorEvents.TableEntryRemoved -= onEntryRemoved;
            }
        }
    }
}
