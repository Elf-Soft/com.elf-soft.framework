using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;

namespace ElfSoft.InventorySystem.Editor
{
    internal static class Utility
    {
        private static readonly Dictionary<int, ItemInfo> itemInfoDic = new();
        private static readonly Dictionary<int, IEnumerable<string>> infoContentDic = new();
        public static IReadOnlyDictionary<int, ItemInfo> ItemInfoDic => itemInfoDic;


        static Utility()
        {
            Reload();
        }

        public static void Reload()
        {
            itemInfoDic.Clear();
            infoContentDic.Clear();
            var itemDatas = AssetDatabase.FindAssets($"t:{nameof(ItemInfoData)}");
            foreach (var guid in itemDatas)
            {
                var asset = AssetDatabase.LoadAssetAtPath<ItemInfoData>(AssetDatabase.GUIDToAssetPath(guid));
                foreach (var i in asset.Entries)
                {
                    itemInfoDic.Add(i.Id, i);
                    infoContentDic.Add(i.Id, GetInfoContent(i));
                }
            }

            //获取信息内容文本,用于匹配搜索
            static IEnumerable<string> GetInfoContent(ItemInfo info)
            {
                yield return info.Id.ToString();
                if (info.MaxStack > 1) yield return info.MaxStack.ToString();

                //获取所有本地化语言名称
                var col = LocalizationEditorSettings.GetStringTableCollection(FieldName.ItemName);
                foreach (var st in col.StringTables)
                {
                    var tableEntry = st.GetEntry(info.Id.ToString());
                    if (tableEntry != null) yield return tableEntry.LocalizedValue;
                }

                //获取所有本地化语言描述
                var dCol = LocalizationEditorSettings.GetStringTableCollection(FieldName.ItemDescription);
                foreach (var st in dCol.StringTables)
                {
                    var tableEntry = st.GetEntry(info.Id.ToString());
                    if (tableEntry != null) yield return tableEntry.LocalizedValue;
                }
            }
        }

        public static ItemInfo GetItemInfo(int id)
        {
            itemInfoDic.TryGetValue(id, out var itemInfo);
            return itemInfo;
        }

        public static IEnumerable<string> GetInfoContent(int id)
        {
            return infoContentDic.ContainsKey(id) ? infoContentDic[id] : null;
        }
    }

    internal static class FieldName
    {
        public static readonly string id = "id";
        public static readonly string name = "name";
        public static readonly string ItemName = "ItemName";
        public static readonly string ItemDescription = "ItemDescription";
    }
}
