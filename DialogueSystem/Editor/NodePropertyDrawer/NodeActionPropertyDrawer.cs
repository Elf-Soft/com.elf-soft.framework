using ElfSoft.Framework;
using UnityEngine.UIElements;

namespace ElfSoft.DialogueSystem.Editor
{
    internal class NodeActionPropertyDrawer : NodePropertyDrawer
    {
        public IntegerField NextField { get; private set; }
        public new NodeAction Node => base.Node as NodeAction;


        public NodeActionPropertyDrawer()
        {
            NextField = new(nameof(NodeAction.Next)) { isDelayed = true };
            NextField.labelElement.pickingMode = PickingMode.Ignore;
            NextField.RegisterValueChangedCallback(e =>
            {
                if (e.newValue == Node.Id) NextField.SetValueWithoutNotify(e.previousValue);
                else if (e.newValue != e.previousValue)
                {
                    EditorView.Asset.SetNodeNext(Node, e.newValue);
                    EventHub.SendEvent<EventData<Node>>(e => e.Init(this, Node));
                }
            });
            Add(NextField);
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            NextField.SetValueWithoutNotify(Node.Next);
        }
    }
}
