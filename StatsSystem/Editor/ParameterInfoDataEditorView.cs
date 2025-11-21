using ElfSoft.Framework.Editor;
using ElfSoft.Framework.Editor.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElfSoft.StatsSystem.Editor
{
    internal sealed class ParameterInfoDataEditorView : TableView<ParameterInfoData>
    {
        public TablePanelController TableController { get; private set; }


        public ParameterInfoDataEditorView()
        {
            TableController = new(this.Q<VisualElement>("left-panel"), nameof(ParameterInfo))
            {
                GetItemSource = () => Asset != null ? Asset.Infos as IList : null,
                BindItem = (elem, index) =>
                {
                    var e = elem as EntryView;
                    var idProperty = So.FindProperty($"infos.Array.data[{index}].id");
                    var nameProperty = So.FindProperty($"infos.Array.data[{index}].abbr");
                    e.BindProperty(idProperty, nameProperty);
                },
                AddItemToSource = () => So.AddArrayElement("infos", p =>
                {
                    p.FindPropertyRelative("id").intValue = So.FindProperty("infos").arraySize - 1;
                    p.FindPropertyRelative("abbr").stringValue = "new item";
                }),
                RemoveItemFromSource = (index) => So.DeleteArrayElementAtIndex("infos", index),
                CheckAsset = CheckAsset

            };

            var PropertiesPanel = this.Q<VisualElement>("right-panel").Q<VisualElement>("body");
            TableController.SelectedIndicesChanged += index => EditorUtils.ShowProperties(PropertiesPanel, So.FindProperty($"infos.Array.data[{index}]"));

            Menu.menu.AppendAction("Sort By Id", SortById, CheckAsset);
            Menu.menu.AppendAction("Copy To text", CopyText, CheckAsset);
            Menu.menu.AppendAction("Create C# enum", CreateEnum, CheckAsset);
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            TableController.UpdateView();
        }

        private void SortById(DropdownMenuAction a)
        {
            (Asset.Infos as List<ParameterInfo>).Sort((x, y) => x.Id.CompareTo(y.Id));
            UpdateView();
        }

        /// <summary>
        /// 将内容转换成enum格式
        /// </summary>
        private string InfosToEnumText()
        {
            StringBuilder sb = new();
            foreach (var data in Asset.Infos)
            {
                var words = data.Abbr.Split(' ');
                string newWord = string.Empty;
                foreach (string word in words)
                {
                    if (word.Length < 2) continue;
                    newWord += char.ToUpper(word[0]) + word[1..].ToLower();
                }
                var entry = $"{newWord} = {data.Id},";
                sb.AppendLine(entry);
            }
            return sb.ToString();
        }

        private void CopyText(DropdownMenuAction _)
        {
            GUIUtility.systemCopyBuffer = InfosToEnumText();
        }

        private void CreateEnum(DropdownMenuAction _)
        {
            var lines = InfosToEnumText().Split('\n');
            StringBuilder sb = new();
            sb.AppendLine("namespace ElfSoft.StatsSystem");
            sb.AppendLine("{");
            sb.AppendLine("\tpublic enum ParameterEnum");
            sb.AppendLine("\t{");
            for (int i = 0; i < lines.Length; i++)
            {
                if (!string.IsNullOrWhiteSpace(lines[i])) sb.AppendLine($"\t\t{lines[i].Trim()}");
            }
            sb.AppendLine("\t}");
            sb.AppendLine("}");

            var defDirectory = @$"Packages/com.elf-soft.framework/StatsSystem/Runtime/";
            var path = EditorUtility.SaveFilePanel("Select directory", defDirectory, "ParameterEnum", "cs");
            if (path.Length > 0)
            {
                File.WriteAllText(path, sb.ToString());
                AssetDatabase.Refresh();
            }
        }
    }
}
