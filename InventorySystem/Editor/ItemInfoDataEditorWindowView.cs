using ElfSoft.Framework.Editor;
using ElfSoft.Framework.Editor.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ElfSoft.InventorySystem.Editor
{
    public class ItemInfoDataEditorWindowView : GameDataView<ItemInfoData>
    {
        public TablePanelController TableController { get; private set; }
        private static readonly string entries = "entries";


        public ItemInfoDataEditorWindowView()
        {
            TableController = new(this.Q<VisualElement>("left-panel"), nameof(ItemInfoData))
            {
                GetItemSource = () => Asset != null ? Asset.Entries as IList : null,
                MakeItem = () => new ItemInfoView(),
                BindItem = (elem, index) =>
                {
                    var e = elem as ItemInfoView;
                    var property = So.FindProperty($"{entries}.Array.data[{index}]");
                    e.BindProperty(property);
                },
                AddItemToSource = () => So.AddArrayElement(entries, p =>
                {
                    p.FindPropertyRelative(FieldName.id).intValue = So.FindProperty(entries).arraySize - 1;
                }),
                RemoveItemFromSource = (index) => So.DeleteArrayElementAtIndex(entries, index),
                CheckAsset = CheckAsset

            };

            var PropertiesPanel = this.Q<VisualElement>("right-panel").Q<VisualElement>("body");
            TableController.SelectedIndicesChanged += index => EditorUtils.ShowProperties(PropertiesPanel, So.FindProperty($"{entries}.Array.data[{index}]"));

            Menu.menu.AppendAction("Sort by Id", SortById, CheckAsset);
            //Menu.menu.AppendAction("Show Name Tables Window", a => LocalizationEditorEx.ShowTableWindow(Utility.ItemNameTable));
            //Menu.menu.AppendAction("Show Info Tables Window", a => LocalizationEditorEx.ShowTableWindow(Utility.ItemDescriptionTable));
            //Menu.menu.AppendAction("UpdateLocalizationKeys", UpdateLocalizationKeys);
            Menu.menu.AppendAction("Reload ItemData", a => Utility.Reload());
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            TableController.UpdateView();
        }

        private void SortById(DropdownMenuAction a)
        {
            (Asset.Entries as List<ItemInfo>).Sort((x, y) => x.Id.CompareTo(y.Id));
            UpdateView();
        }

        //private void UpdateLocalizationKeys(DropdownMenuAction action)
        //{
        //    ItemEditorDatabase.Reload();
        //    List<string> keys = new();
        //    foreach (var i in ItemEditorDatabase.ItemInfoDic.Values)
        //    {
        //        keys.Add(i.Id.ToString());
        //    }
        //    LocalizationEditorEx.UpdateStringTableKeysById(Utility.ItemNameTable, keys);
        //    LocalizationEditorEx.UpdateStringTableKeysById(Utility.ItemDescriptionTable, keys);
        //}
    }
}
