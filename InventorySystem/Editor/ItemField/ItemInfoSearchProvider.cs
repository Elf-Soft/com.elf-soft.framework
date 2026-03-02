using ElfSoft.Framework.Editor;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEngine;

namespace ElfSoft.InventorySystem.Editor
{
    public class ItemInfoSearchProvider : SearchProvider
    {
        private QueryEngine<ItemInfo> QueryEngine { get; } = new();
        private static readonly Texture2D noneIcon = EditorGUIUtility.FindTexture("Search Icon");


        public ItemInfoSearchProvider() : base(nameof(Item).ToLower())
        {
            fetchItems = FetchItems;
            //设置SearchItem缩略图
            fetchThumbnail = (si, sc) =>
            {
                var icon = (si.data as ItemInfo).Icon?.editorAsset as Sprite;
                return icon != null ? icon.texture : noneIcon;
            };

            //获取搜索物品的可迭代文本,用于根据搜索输入栏文本(非过滤器)筛选数据集
            QueryEngine.SetSearchDataCallback(info => Utility.GetInfoContent(info.Id), StringComparison.OrdinalIgnoreCase);
            AddFilters(QueryEngine);

            fetchPropositions = FetchPropositions;
        }

        [SearchItemProvider]
        internal static SearchProvider CreateProvider()
        {
            return new ItemInfoSearchProvider();
        }

        //获取目标数据集
        private IEnumerator FetchItems(SearchContext sc, List<SearchItem> items, SearchProvider provider)
        {
            if (sc.empty) yield break;

            var query = QueryEngine.ParseQuery(sc.searchQuery);
            if (!query.valid) yield break;

            //从数据缓存中获取数据
            var filteredObjects = query.Apply(Utility.ItemInfoDic.Values);
            foreach (var info in filteredObjects)
            {
                var id = info.Id.ToString();
                var name = LocalizationEditorEx.GetStringTableEntryLocalizedValue(FieldName.ItemName, id);
                var description = LocalizationEditorEx.GetStringTableEntryLocalizedValue(FieldName.ItemDescription, id);
                //Texture2D preview = AssetPreview.GetAssetPreview(data);
                yield return provider.CreateItem(sc, id, name, description, null, info);
            }
        }

        //添加过滤器
        private static void AddFilters(QueryEngine<ItemInfo> queryEngine)
        {
            queryEngine.AddFilter(nameof(ItemInfo.Id).ToLower(), i => i.Id);
            queryEngine.AddFilter(nameof(ItemInfo.MaxStack).ToLower(), i => i.MaxStack);

            //queryEngine.AddFilter(FieldName.TranslatedValue,
            //    filterResolver: (ItemInfo d, string filterNameMatch, string operatorToken, string filterValue) =>
            //    {
            //        Debug.Log($"{filterNameMatch} - {operatorToken} - {filterValue}");
            //        return false;
            //    });
        }

        //创建默认可选过滤器菜单
        protected virtual IEnumerable<SearchProposition> FetchPropositions(SearchContext context, SearchPropositionOptions options)
        {
            // Only show propositions when our provider is being used, not for general searches.
            //仅当前SearchProvider可用
            if (!options.flags.HasAny(SearchPropositionFlags.QueryBuilder) || context.filterId != filterId)
                yield break;

            yield return new SearchProposition(label: nameof(ItemInfo.Id).ToLower(), $"{nameof(ItemInfo.Id).ToLower()}=0", "Filter by item ID.");
            yield return new SearchProposition(label: nameof(ItemInfo.MaxStack).ToLower(), $"{nameof(ItemInfo.MaxStack).ToLower()}=0", "Filter by item MaxStack.");

            //yield return new SearchProposition("Translated", "Any", $"tr:\"some value\"",
            //    "Filter by Table localized value.");

            //foreach (var locale in LocalizationEditorSettings.GetLocales())
            //{
            //    yield return new SearchProposition("Translated", locale.LocaleName,
            //        $"tr({locale.Identifier.Code}):\"some value\"",
            //        $"Filter by {locale.LocaleName} Table localized value.", icon: EditorGUIUtility.FindTexture("Packages/com.unity.localization/Editor/Icons/" + "Locale/Locale.png"));
            //}
        }

    }
}
