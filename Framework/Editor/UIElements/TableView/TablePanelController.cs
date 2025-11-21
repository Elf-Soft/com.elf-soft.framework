using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElfSoft.Framework.Editor.UIElements
{
    public partial class TablePanelController
    {
        public VisualElement PanelElement { get; private set; }
        public ListView ListView { get; private set; }
        public Label TitleLabel => PanelElement.Q<Label>("title-label");
        public Button AddButton => PanelElement.Q<Button>("add-button");
        public VisualElement ClickArea => PanelElement.Q<VisualElement>("click-area");


        public TablePanelController(VisualElement panelElement)
        {
            PanelElement = panelElement;
            ListView = panelElement.Q<ListView>();
            ListView.style.flexGrow = 1;
            ListView.selectionType = SelectionType.Multiple;
        }
    }

    public partial class TablePanelController
    {
        public Func<IList> GetItemSource { get; set; }
        public Func<VisualElement> MakeItem { get; set; }
        public Action<VisualElement, int> BindItem { get; set; }
        public Action<VisualElement, int> UnbindItem { get; set; }
        public Action AddItemToSource { get; set; }
        public Action<int> RemoveItemFromSource { get; set; }
        public Func<DropdownMenuAction, DropdownMenuAction.Status> CheckAsset { get; set; }
        private ContextualMenuManipulator listViewMenuManipulator;
        public ContextualMenuManipulator ListViewMenuManipulator
        {
            get => listViewMenuManipulator;
            set
            {
                ListView.RemoveManipulator(listViewMenuManipulator);
                listViewMenuManipulator = value;
                ListView.AddManipulator(listViewMenuManipulator);
            }
        }
        public event Action<int> SelectedIndicesChanged;


        public TablePanelController(VisualElement panelElement, string title = null) : this(panelElement)
        {
            if (!string.IsNullOrEmpty(title)) TitleLabel.text = title;

            ListView.makeItem = () => MakeItem();
            ListView.bindItem = (e, i) => BindItem(e, i);
            ListView.unbindItem = (e, i) => UnbindItem(e, i);
            ListView.selectedIndicesChanged += e => SelectedIndicesChanged?.Invoke(ListView.selectedIndex);
            ListView.itemIndexChanged += (e, i) => ListView.SetSelection(i);
            ListView.RegisterCallback<KeyDownEvent>(evt => { if (evt.keyCode == KeyCode.Delete) RemoveItems(); });
            ListViewMenuManipulator = new ContextualMenuManipulator(evt =>
            {
                evt.menu.AppendAction("Add", a => AddItem(), CheckAsset);
                evt.menu.AppendAction("Remove", a => RemoveItems(), CheckAsset);
            });

            MakeItem = () => new EntryView();
            UnbindItem = (elem, index) => (elem as EntryView).Unbind();
            PanelElement.Q<Button>("add-button").clicked += AddItem;

        }

        public void UpdateView()
        {
            var source = GetItemSource();
            ListView.itemsSource = source;
            ListView.RefreshItems();

            if (source == null) return;
            if (ListView.selectedIndex < 0 && source.Count > 0) ListView.SetSelection(0);
            else SelectedIndicesChanged?.Invoke(ListView.selectedIndex);
        }

        private void AddItem()
        {
            AddItemToSource();
            ListView.RefreshItems();
            if (ListView.itemsSource != null) ListView.SetSelection(ListView.itemsSource.Count - 1);
        }

        protected void RemoveItems()
        {
            if (ListView.selectedItem == null) return;
            var list = ListView.selectedIndices.ToList();
            list.Sort();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                RemoveItemFromSource(list[i]);
                if (list[i] == ListView.selectedIndex) SelectedIndicesChanged?.Invoke(ListView.selectedIndex);
            }
            ListView.RefreshItems();
        }
    }
}
