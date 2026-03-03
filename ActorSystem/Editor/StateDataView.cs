using ElfSoft.Framework.Editor;
using ElfSoft.Framework.Editor.UIElements;
using System;
using System.Collections;
using UnityEngine.UIElements;

namespace ElfSoft.ActorSystem.StateSystem.Editor
{
    [GameDataEditorGenerator(typeof(StateData), true)]
    public class StateDataView : GameDataView<StateData>
    {
        public TablePanelController TableController { get; private set; }


        public StateDataView()
        {
            TableController = new(this.Q<VisualElement>("left-panel"), nameof(StateData))
            {
                GetItemSource = () => Asset != null ? Asset.States as IList : null,
                //MakeItem = () => new ItemInfoView(),
                BindItem = (elem, i) =>
                {
                    var e = elem as GameDataEntryBar;
                    var s = Asset.States[i];
                    e.IdLabel.text = i.ToString();
                    e.NameLabel.text = s.GetType().Name;
                },
                AddItemToSource = () => So.AddArrayElement("states"),
                RemoveItemFromSource = (index) => So.DeleteArrayElementAtIndex("states", index),
                ListViewMenuManipulator = new ContextualMenuManipulator(evt =>
                {
                    CreateAddReferenceItemMenuActions<State>(evt.menu, "Add/");
                    evt.menu.AppendAction("Remove", a => TableController.RemoveItems(), CheckAsset);
                }),
                CheckAsset = CheckAsset

            };

            var PropertiesPanel = this.Q<VisualElement>("right-panel").Q<VisualElement>("body");
            TableController.SelectedIndicesChanged += index => EditorUtils.ShowProperties(PropertiesPanel, So.FindProperty($"states.Array.data[{index}]"));

            //SetButtonLeftClickMenu
            TableController.AddButton.clickable = null;
            var cm = new ContextualMenuManipulator(e => CreateAddReferenceItemMenuActions<State>(e.menu, string.Empty));
            cm.activators.Clear();
            cm.activators.Add(new ManipulatorActivationFilter { button = MouseButton.LeftMouse });
            TableController.AddButton.AddManipulator(cm);
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            TableController.UpdateView();
        }

        private void CreateAddReferenceItemMenuActions<T>(DropdownMenu menu, string rootPath) where T : class
        {
            var datas = TypeMenuUtility.GetSubTypeTypeMenuDatas(typeof(T));
            foreach (var data in datas)
            {
                CreateMenuAction(data.path, data.name, data.type);
            }

            void CreateMenuAction(string path, string name, Type type)
            {
                menu.AppendAction(rootPath + path + name, a =>
                {
                    So.AddArrayReferenceElement("states", type);
                    TableController.UpdateView();
                }, CheckAsset);
            }
        }

    }
}