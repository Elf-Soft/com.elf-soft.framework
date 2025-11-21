using ElfSoft.Framework;
using ElfSoft.Framework.Editor;
using ElfSoft.Framework.Editor.UIElements;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Localization.Tables;
using UnityEngine.UIElements;

namespace ElfSoft.InventorySystem.Editor
{
    public class ItemInfoView : EntryView
    {
        private SerializedProperty property;
        private readonly TextField textField;


        public ItemInfoView()
        {
            textField = new();
            textField.style.visibility = Visibility.Hidden;
            textField.RegisterValueChangedCallback(e =>
            {
                if (e.newValue != e.previousValue) UpdateView();
            });
            this.Q<VisualElement>("item-row").Add(textField);

            LocalizationEditorEx.RegisterEditorEntryChangedEvent(this, OnEntryModified, OnEntryRemoved);
        }

        #region LocalizationEditorEvents
        private void OnEntryRemoved(LocalizationTableCollection collection, SharedTableData.SharedTableEntry entry)
        {
            if (entry.Key == IdLabel.text) UpdateView();
        }

        private void OnEntryModified(SharedTableData.SharedTableEntry entry)
        {
            if (entry.Key == IdLabel.text) UpdateView();
        }

        #endregion

        public virtual void BindProperty(SerializedProperty property)
        {
            this.property = property;
            IdLabel.BindProperty(property.FindPropertyRelative(FieldName.id));
            textField.BindProperty(property.FindPropertyRelative(FieldName.name));
            UpdateView();
        }

        public virtual void UpdateView()
        {
            var value = property.FindPropertyRelative(FieldName.name).stringValue;
            Utils.SplitLocalText(value, out var tableName, out var entryKey);
            NameLabel.text = LocalizationEditorEx.GetStringTableEntryLocalizedValue(tableName, entryKey);
        }

    }
}
