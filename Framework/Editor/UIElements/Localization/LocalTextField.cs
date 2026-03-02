using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Localization;
using UnityEditor.Localization.Search;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using UnityEngine.Search;
using UnityEngine.UIElements;
using Ex = ElfSoft.Framework.Editor.LocalizationEditorEx;

namespace ElfSoft.Framework.Editor.UIElements
{
    public class LocalTextField : DropdownFoldout
    {
        private readonly VisualElement container;
        private readonly DropdownField LocaleField;
        private readonly List<string> choices = new();
        private readonly TextField textField;
        private static readonly SearchViewState state = GetSearchViewState();
        private string tableName;
        private string entryKey;
        public StyleLength TextElementMinHeight
        {
            get => textField.style.minHeight;
            set => textField.style.minHeight = value;
        }
        public StyleLength TextElementMaxHeight
        {
            get => textField.style.maxHeight;
            set => textField.style.maxHeight = value;
        }
        public LocaleIdentifier Identifier => LocalizationEditorSettings.GetLocales()[LocaleField.index].Identifier;


        public LocalTextField()
        {
            container = new();
            container.style.flexDirection = FlexDirection.Row;
            Foldout.Add(container);

            LocaleField = new(nameof(Locale));
            LocaleField.style.flexGrow = 1;
            LocaleField.style.flexShrink = 1;
            LocaleField.Q<Label>().style.minWidth = 105;
            LocaleField.RegisterValueChangedCallback(e => UpdateView());
            LocaleField.AddToClassList(DropdownField.alignedFieldUssClassName);
            container.Add(LocaleField);

            Button btn = new();
            btn.style.minWidth = 32;
            btn.focusable = false;
            btn.style.backgroundImage = Ex.TableWindow;
            btn.style.backgroundSize = BackgroundPropertyHelper.ConvertScaleModeToBackgroundSize(ScaleMode.ScaleToFit);
            btn.clicked += () => Ex.ShowEntryWindow(tableName, entryKey);
            container.Add(btn);

            textField = new()
            {
                //selectAllOnMouseUp = false,
                //isDelayed = true,
                multiline = true,
            };
            textField.RegisterValueChangedCallback(e =>
            {
                if (e.newValue != e.previousValue && Ex.TryGetStringTableEntry(tableName, entryKey, Identifier, out var result))
                {
                    Undo.RecordObject(result.entry.Table, "Change string table entry value");
                    result.entry.Value = e.newValue;
                    EditorUtility.SetDirty(result.table);
                }
            });
            Foldout.Add(textField);


            Dropdown.RegisterValueChangedCallback(e =>
            {
                if (e.newValue == e.previousValue) return;
                LocalizationEx.SplitLocalText(e.newValue, out tableName, out entryKey);
                if (string.IsNullOrEmpty(e.newValue)) Dropdown.SetValueWithoutNotify(Ex.NoTableEntrySelected);
                UpdateView();
            });
            var clickArea = Dropdown.Q<TextElement>().parent;
            clickArea.style.flexGrow = 1;
            clickArea.focusable = false;
            clickArea.pickingMode = PickingMode.Position;
            var cm = new Clickable(e => ShowPicker());
            cm.activators.Clear();
            cm.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            clickArea.AddManipulator(cm);

            Ex.RegisterEditorEntryChangedEvent(this, OnEntryModified, OnEntryRemoved);
            UpdateLocal();
        }

        #region LocalizationEditorEvent

        private void OnEntryModified(SharedTableData.SharedTableEntry entry)
        {
            if (entry.Key == entryKey) UpdateView();
        }

        private void OnEntryRemoved(LocalizationTableCollection collection, SharedTableData.SharedTableEntry entry)
        {
            if (entry.Key == entryKey) UpdateView();
        }

        #endregion

        public override void BindProperty(SerializedProperty property)
        {
            base.BindProperty(property);
            Dropdown.Unbind();
            Dropdown.BindProperty(property);
        }

        public void BindString(string value)
        {
            LocalizationEx.SplitLocalText(value, out tableName, out entryKey);
            Dropdown.SetValueWithoutNotify(value);
            UpdateView();
        }

        private void UpdateLocal()
        {
            var locales = LocalizationEditorSettings.GetLocales();
            if (locales.Count > 0)
            {
                choices.Clear();
                foreach (var l in locales)
                {
                    choices.Add(l.LocaleName);
                }
                LocaleField.choices = choices;
                var index = Ex.TryGetSelectedOrDefaultLocale(out var locale) ? locales.IndexOf(locale) : 0;
                LocaleField.index = index;
            }
        }

        private void UpdateView()
        {
            var b = Ex.TryGetStringTableEntry(tableName, entryKey, Identifier, out var result);
            textField.SetValueWithoutNotify(b ? result.entry.Value : Ex.NoTableEntrySelected);
            textField.enabledSelf = b;
        }

        #region Search
        private void ShowPicker()
        {
            state.trackingHandler = TrackingHandler;
            state.selectHandler = static (i, b) => { };
            SearchService.ShowPicker(state);
        }

        private void TrackingHandler(SearchItem item)
        {
            if (item.data is TableEntrySearchData sd)
            {
                tableName = sd.Collection.SharedData.TableCollectionName;
                entryKey = sd.Entry.Key;
                var value = $"{tableName}/{entryKey}";
                if (property != null)
                {
                    property.stringValue = value;
                    property.serializedObject.ApplyModifiedProperties();
                }
                else Dropdown.value = value;
            }
        }

        private static SearchViewState GetSearchViewState()
        {
            var context = SearchService.CreateContext(Ex.GetStringTableSearchProvider(), Ex.StringTableProviderFilter);
            return new SearchViewState(context)
            {
                title = "string table entry",
                excludeClearItem = false,
                ignoreSaveSearches = true,
                hideAllGroup = false,
                queryBuilderEnabled = true,
                hideTabs = true,
                flags = SearchViewFlags.DisableInspectorPreview | SearchViewFlags.OpenInBuilderMode,
            };
        }

        #endregion
    }
}
