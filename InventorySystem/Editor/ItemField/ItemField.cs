using ElfSoft.Framework.Editor;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEngine.Search;
using UnityEngine.UIElements;

namespace ElfSoft.InventorySystem.Editor
{
    public class ItemField : BaseField<Item>
    {
        #region Field
        private readonly Label nameLabel;
        private readonly Label textLabel;
        private readonly VisualElement selector;
        private readonly Image icon;
        private static int copybuffer = -1;
        private static readonly string searchText = "item:";
        private static readonly string emptyItemText = "<null>";
        private static readonly string objectUssClassName = "unity-object-field";
        private static readonly string objectInputUssClassName = objectUssClassName + "__input";
        private static readonly string objectObjectUssClassName = objectUssClassName + "__object";
        private static readonly string objectSelectorUssClassName = objectUssClassName + "__selector";
        private static readonly string displayUssClassName = "unity-object-field-display";
        private static readonly string displayIconUssClassName = displayUssClassName + "__icon";
        private static readonly string displayLabelUssClassName = displayUssClassName + "__label";
        private static readonly string displayLabelNullUssClassName = displayLabelUssClassName + "--value-null";

        private static readonly SearchViewState searchViewState = GetSearchViewState();
        private SerializedProperty property;
        private int PropertyValue => property.FindPropertyRelative(FieldName.id).intValue;

        #endregion


        public ItemField() : base(nameof(ItemField), null)
        {
            AddToClassList(alignedFieldUssClassName);
            nameLabel = this.Q<Label>(null, labelUssClassName);
            var content = this.Q<VisualElement>(null, inputUssClassName);
            content.AddToClassList(objectInputUssClassName);
            VisualElement display = new();
            display.AddToClassList(objectObjectUssClassName);
            content.Add(display);

            icon = new();
            icon.AddToClassList(displayIconUssClassName);
            display.Add(icon);

            textLabel = new();
            textLabel.AddToClassList(displayLabelUssClassName);
            textLabel.AddToClassList(displayLabelNullUssClassName);
            display.Add(textLabel);

            selector = new();
            selector.AddToClassList(objectSelectorUssClassName);
            content.Add(selector);

            Clickable clickable = new(ShowPicker);
            selector.AddManipulator(clickable);
            this.AddManipulator(new ContextualMenuManipulator(evt =>
            {
                var e = evt.menu;
                evt.menu.AppendAction("Reset", a => ApplyPropertyValue(-1),
                    a => PropertyValue < 0 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
                evt.menu.AppendAction("Copy", a => copybuffer = PropertyValue,
                    a => PropertyValue < 0 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
                evt.menu.AppendAction("Paste", a => ApplyPropertyValue(PropertyValue),
                a => copybuffer < 0 ? DropdownMenuAction.Status.Disabled : DropdownMenuAction.Status.Normal);
                evt.menu.AppendSeparator();
                evt.menu.AppendAction("Copy Property Path", a => GUIUtility.systemCopyBuffer = property.propertyPath);
            }));

            LocalizationEditorEx.RegisterEditorEntryChangedEvent(this, OnEntryModified, OnEntryRemoved);
            this.RegisterUndoEvent(UpdateView);
        }

        #region EditorEvents

        private void OnEntryRemoved(LocalizationTableCollection collection, SharedTableData.SharedTableEntry entry)
        {
            if (entry.Key == PropertyValue.ToString()) ResetView();
        }

        private void OnEntryModified(SharedTableData.SharedTableEntry entry)
        {
            if (entry.Key == PropertyValue.ToString()) UpdateView();
        }

        #endregion

        public void BindProperty(SerializedProperty property)
        {
            this.property = property;
            nameLabel.text = property.displayName;
            UpdateView();
        }

        private void ShowItemInfo(ItemInfo itemInfo)
        {
            if (itemInfo != null)
            {
                var id = itemInfo.Id.ToString();
                var name = LocalizationEditorEx.GetStringTableEntryLocalizedValue(FieldName.ItemName, id);
                textLabel.text = $"{name}({id})";
                textLabel.RemoveFromClassList(displayLabelNullUssClassName);
                icon.image = itemInfo.Icon?.editorAsset is Sprite s ? s.texture : null;
            }
            else ResetView();
        }

        private void UpdateView()
        {
            property.serializedObject.Update();
            var value = PropertyValue;
            if (value >= 0) ShowItemInfo(Utility.GetItemInfo(value));
            else ResetView();
        }

        private void ResetView()
        {
            textLabel.text = emptyItemText;
            textLabel.AddToClassList(displayLabelNullUssClassName);
            icon.image = null;
        }

        private void ApplyPropertyValue(int value)
        {
            if (property == null) return;
            property.FindPropertyRelative(FieldName.id).intValue = value;
            property.serializedObject.ApplyModifiedProperties();
        }

        #region Search
        private void ShowPicker()
        {
            searchViewState.trackingHandler = TrackingHandler;
            searchViewState.selectHandler = SelectHandler;
            SearchService.ShowPicker(searchViewState);
        }

        private void TrackingHandler(SearchItem si)
        {
            ShowItemInfo(si.data as ItemInfo);
        }

        private void SelectHandler(SearchItem si, bool arg2)
        {
            ApplyPropertyValue(si.data is ItemInfo info ? info.Id : -1);
        }

        private static SearchViewState GetSearchViewState()
        {
            var context = SearchService.CreateContext(new ItemInfoSearchProvider(), searchText);
            var state = new SearchViewState(context, SearchViewFlags.GridView | SearchViewFlags.OpenInBuilderMode)
            {
                title = nameof(Item),
                ignoreSaveSearches = true,
                hideAllGroup = false,
                queryBuilderEnabled = true,
                hideTabs = true,

            };
            return state;
        }

        #endregion
    }
}
