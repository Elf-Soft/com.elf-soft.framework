using ElfSoft.Framework;
using ElfSoft.Framework.Editor;
using ElfSoft.Framework.Editor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

namespace ElfSoft.DialogueSystem.Editor
{
    internal class ChoiceView : VisualElement
    {
        public IntegerField IdLabel { get; private set; }
        public LocalTextField TextField { get; private set; }
        public IntegerField NextField { get; private set; }
        public NodeChoice Node { get; internal set; }
        public Option Opt { get; internal set; }
        public DialogueDataView EditorView { get; internal set; }


        public ChoiceView()
        {
            style.marginTop = 4;
            style.paddingLeft = 16;
            style.paddingRight = 4;
            style.paddingBottom = 4;

            IdLabel = new(nameof(Option.Id));
            IdLabel.labelElement.pickingMode = PickingMode.Ignore;
            IdLabel.style.paddingTop = 4;
            IdLabel.SetEnabled(false);
            Add(IdLabel);

            NextField = new(nameof(Option.Next)) { isDelayed = true };
            NextField.labelElement.pickingMode = PickingMode.Ignore;
            NextField.RegisterValueChangedCallback(e =>
            {
                if (e.newValue == IdLabel.value) NextField.SetValueWithoutNotify(e.previousValue);
                else if (e.newValue != e.previousValue)
                {
                    EditorView.Asset.SetOptionNext(Opt, e.newValue);
                    EventHub.SendEvent<EventData<(Node, Option)>>(e => e.Init(this, (Node, Opt)));
                }
            });
            Add(NextField);

            TextField = new() { Title = nameof(Option.Text), TextElementMaxHeight = 60 };
            TextField.Dropdown.RegisterValueChangedCallback(e =>
            {
                if (e.newValue == e.previousValue) return;
                Undo.RecordObject(EditorView.Asset, $"Set option text to ({e.newValue})");
                ReflectionEx.SetFieldValue(Opt, FieldName.text, e.newValue);
            });
            Add(TextField);


            RegisterCallback<AttachToPanelEvent>(e =>
            {
                EventHub.AddListener<EventData<Node>>(OnNodeEdited);
                EventHub.AddListener<EventData<(Node, Option)>>(OnPortEdited);
            });
            RegisterCallback<DetachFromPanelEvent>(e =>
            {
                EventHub.RemoveListener<EventData<Node>>(OnNodeEdited);
                EventHub.RemoveListener<EventData<(Node, Option)>>(OnPortEdited);
            });
        }

        protected virtual void OnNodeEdited(EventData<Node> e)
        {
            if (e.Args == Node && e.Sender != this) UpdateTextField();
        }

        private void OnPortEdited(EventData<(Node node, Option choice)> e)
        {
            if (e.Args.choice == Opt && e.Sender != this) NextField.SetValueWithoutNotify(e.Args.choice.Next);
        }

        public void Bind(DialogueDataView editorView, NodeChoice node, Option opt, int index)
        {
            EditorView = editorView;
            Node = node;
            Opt = opt;
            IdLabel.SetValueWithoutNotify(index);
            UpdateTextField();
            NextField.SetValueWithoutNotify(opt != null ? opt.Next : -1);
        }

        public void Unbind()
        {
            EditorView = null;
            Node = null;
            Opt = null;
        }

        private void UpdateTextField() => TextField.BindString(Opt.Text);
    }
}
