using ElfSoft.Framework;
using System.Collections.Generic;
using UnityEngine.UIElements;

namespace ElfSoft.DialogueSystem.Editor
{
    internal class NodePropertyDrawer : VisualElement
    {
        public IntegerField IdField { get; private set; }
        public Node Node { get; internal set; }
        public DialogueDataEditorWindowView EditorView => userData as DialogueDataEditorWindowView;


        public NodePropertyDrawer()
        {
            IdField = new(nameof(Node.Id)) { isDelayed = true };
            IdField.labelElement.pickingMode = PickingMode.Ignore;
            IdField.RegisterValueChangedCallback(e =>
            {
                if ((EditorView.Asset.Nodes as List<Node>).Find(n => n.Id == e.newValue) != null)
                {
                    IdField.SetValueWithoutNotify(e.previousValue);
                }
                else if (e.newValue != e.previousValue)
                {
                    EditorView.Asset.SetNodeId(Node, e.newValue);
                    EditorView.GraphView.UpdateView();
                }
            });
            Add(IdField);

            RegisterCallback<AttachToPanelEvent>(e => EventHub.AddListener<EventData<Node>>(OnNodeEdited));
            RegisterCallback<DetachFromPanelEvent>(e => EventHub.RemoveListener<EventData<Node>>(OnNodeEdited));
        }

        public void BindNode(Node node)
        {
            Node = node;
            UpdateView();
        }

        protected virtual void UpdateView()
        {
            IdField.SetValueWithoutNotify(Node.Id);
        }

        private void OnNodeEdited(EventData<Node> data)
        {
            if (data.Args == Node && data.Sender != this) UpdateView();
        }
    }
}
