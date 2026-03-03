using ElfSoft.Framework.Editor;
using ElfSoft.Framework.Editor.UIElements;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Search;
using UnityEditor.UIElements;
using UnityEngine.InputSystem;
using UnityEngine.Search;
using UnityEngine.UIElements;

namespace ElfSoft.InputSystemExtension.Editor
{
    [GameDataEditorGenerator(typeof(BindingSettingData), true)]
    public class BindingSettingDataView : GameDataView<BindingSettingData>
    {
        public TablePanelController DataController { get; private set; }
        public TablePanelController GroupController { get; private set; }
        private readonly SearchViewState searchViewState;


        public BindingSettingDataView() : base("UI/UIDocument/ThreePanelTableView")
        {
            DataController = new(this.Q<VisualElement>("left-panel"), nameof(BindingSettingGroup))
            {
                GetItemSource = () => Asset != null ? Asset.Groups as IList : null,
                BindItem = (elem, index) =>
                {
                    var e = elem as GameDataEntryBar;
                    e.IdLabel.text = index.ToString();
                    e.NameLabel.BindProperty(So.FindProperty($"groups.Array.data[{index}].name"));
                },
                AddItemToSource = () => So.AddArrayElement("groups", p => p.FindPropertyRelative("name").stringValue = "new group"),
                RemoveItemFromSource = (index) => So.DeleteArrayElementAtIndex("groups", index),
                CheckAsset = CheckAsset

            };

            GroupController = new(this.Q<VisualElement>("mid-panel"), nameof(BindingSetting))
            {
                GetItemSource = () =>
                {
                    var group = DataController.ListView.selectedItem as BindingSettingGroup;
                    var lsit = Asset.Groups as List<BindingSettingGroup>;
                    return lsit.Contains(group) ? group.Settings as IList : null;
                },
                MakeItem = () => new BindingSettingView(),
                BindItem = (elem, index) =>
                {
                    var e = elem as BindingSettingView;
                    e.BindProperty(So.FindProperty($"groups.Array.data[{DataController.ListView.selectedIndex}].settings.Array.data[{index}]"));
                },
                UnbindItem = (elem, index) => (elem as BindingSettingView).Unbind(),
                AddItemToSource = () => So.AddArrayElement($"groups.Array.data[{DataController.ListView.selectedIndex}].settings", p => p.FindPropertyRelative("name").stringValue = "new setting"),
                RemoveItemFromSource = (index) => So.DeleteArrayElementAtIndex($"groups.Array.data[{DataController.ListView.selectedIndex}].settings", index),
                CheckAsset = CheckAsset
            };
            GroupController.AddButton.style.display = DisplayStyle.Flex;
            DataController.SelectedIndicesChanged += index => GroupController.UpdateView();

            searchViewState = GetSearchViewState();
            Menu.menu.AppendAction("Import From InputActionAsset", a => ShowPicker(), CheckAsset);
            this.RegisterUndoEvent(UpdateView);
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            DataController.UpdateView();
        }

        #region Search
        public void ShowPicker() => SearchService.ShowPicker(searchViewState);

        SearchViewState GetSearchViewState()
        {
            var context = SearchService.CreateContext($"p: t:{nameof(InputActionAsset)}");
            var viewState = new SearchViewState(context)
            {
                title = nameof(InputActionAsset),
                flags = SearchViewFlags.HideSearchBar | SearchViewFlags.GridView | SearchViewFlags.DisableInspectorPreview,
                selectHandler = (searchItem, value) =>
                {
                    if (searchItem.ToObject() is not InputActionAsset asset) return;
                    FindAllInputActionReferenceFromAsset(asset, out var references);
                    AddGroupsToAsset(asset, out var groupDic);
                    AddSettingToAsset(asset, references, groupDic);
                    UpdateView();
                },
            };
            return viewState;
        }

        //获取选择的InputActionAsset的所有InputActionReference
        void FindAllInputActionReferenceFromAsset(InputActionAsset asset, out List<InputActionReference> references)
        {
            var assetPath = AssetDatabase.GetAssetPath(asset);
            var assets = AssetDatabase.LoadAllAssetRepresentationsAtPath(assetPath);
            references = new();
            foreach (var item in assets)
            {
                if (item is InputActionReference reference)
                {
                    references.Add(reference);
                }
            }
        }

        //根据控制器类型添加分组
        void AddGroupsToAsset(InputActionAsset asset, out Dictionary<string, BindingSettingGroup> groupDic)
        {
            groupDic = new();
            var groups = Asset.Groups as List<BindingSettingGroup>;
            groups.Clear();
            for (int i = 0; i < asset.controlSchemes.Count; i++)
            {
                var scheme = asset.controlSchemes[i];
                BindingSettingGroup g = new();
                ReflectionEx.SetFieldValue(g, "name", scheme.name);

                groups.Add(g);
                groupDic[scheme.name] = g;
            }
        }

        //添加Setting
        void AddSettingToAsset(InputActionAsset asset, List<InputActionReference> references, Dictionary<string, BindingSettingGroup> groupDic)
        {
            foreach (var actionMap in asset.actionMaps)
            {
                foreach (var action in actionMap)
                {
                    for (int i = 0; i < action.bindings.Count; i++)
                    {
                        var binding = action.bindings[i];
                        if (binding.isComposite) continue;
                        var groups = binding.groups.Split(InputBinding.Separator);
                        foreach (var groupName in groups)
                        {
                            if (string.IsNullOrEmpty(groupName)) continue;
                            if (groupDic.TryGetValue(groupName, out var settingGroup))
                            {
                                var name = action.name;
                                if (binding.isPartOfComposite) name += $"_{binding.name}";

                                BindingSetting setting = new();
                                var reference = references.Find(r => r.action.name == action.name);
                                //Debug.Log($"{actionMap.name} - {reference} - {action}");
                                ReflectionEx.SetFieldValue(setting, "name", name);
                                ReflectionEx.SetFieldValue(setting, "reference", reference);
                                ReflectionEx.SetFieldValue(setting, "bindingIndex", i);
                                (settingGroup.Settings as List<BindingSetting>).Add(setting);
                            }
                        }
                    }
                }
            }
        }

        #endregion
    }
}
