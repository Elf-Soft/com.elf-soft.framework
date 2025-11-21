using ElfSoft.Framework;
using ElfSoft.Framework.Editor;
using ElfSoft.Framework.Editor.UIElements;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine.UIElements;

namespace ElfSoft.DialogueSystem.Editor
{
    internal class NodeChoicePropertyDrawer : NodePropertyDrawer
    {
        public ListView ListView { get; private set; }
        public LocalTextField TextField { get; private set; }
        public new NodeChoice Node => base.Node as NodeChoice;


        public NodeChoicePropertyDrawer()
        {
            ListView = new()
            {
                showFoldoutHeader = true,
                headerTitle = nameof(NodeChoice.Choices),
                showAddRemoveFooter = true,
                reorderable = true,
                reorderMode = ListViewReorderMode.Animated,
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                showAlternatingRowBackgrounds = AlternatingRowBackground.All,
                showBoundCollectionSize = false,
                makeItem = () => new ChoiceView(),
                bindItem = (elem, i) => (elem as ChoiceView).Bind(EditorView, Node, Node.Choices[i], i),
                unbindItem = (elem, i) => (elem as ChoiceView).Unbind(),
                onAdd = v =>
                {
                    (Node.Choices as List<Option>).Add(new());
                    EventHub.SendEvent<EventData<Node>>(e => e.Init(this, Node));
                },
                onRemove = v =>
                {
                    var choices = Node.Choices as List<Option>;
                    if (ListView.selectedItem != null)
                    {
                        foreach (var obj in ListView.selectedItems.ToList())
                        {
                            choices.Remove(obj as Option);
                            ListView.SetSelection(-1);
                            ListView.RefreshItems();
                            EventHub.SendEvent<EventData<Node>>(e => e.Init(this, Node));
                        }
                    }
                    else if (Node.Choices.Count > 0)
                    {
                        choices.RemoveAt(Node.Choices.Count - 1);
                        ListView.RefreshItems();
                        EventHub.SendEvent<EventData<Node>>(e => e.Init(this, Node));
                    }
                },
            };
            ListView.itemIndexChanged += (x, y) => EventHub.SendEvent<EventData<Node>>(e => e.Init(this, Node));
            Add(ListView);

            TextField = new()
            {
                Title = nameof(NodeText.Text),
                TextElementMinHeight = 120f,
                TextElementMaxHeight = Length.Percent(80)
            };
            TextField.Dropdown.RegisterValueChangedCallback(e =>
            {
                if (e.newValue == e.previousValue) return;
                Undo.RecordObject(EditorView.Asset, $"Set node text to ({e.newValue})");
                ReflectionEx.SetFieldValue(Node, FieldName.text, e.newValue);
            });
            TextField.style.marginTop = 12f;
            Add(TextField);

            IdField.RegisterValueChangedCallback(e =>
            {
                if (e.newValue != e.previousValue)
                {
                    ListView.RefreshItems();
                    UpdateTextField();
                }
            });
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            ListView.itemsSource = Node.Choices as IList;
            ListView.RefreshItems();
            UpdateTextField();
        }

        private void UpdateTextField() => TextField.BindString(Node.Text);

    }
}
