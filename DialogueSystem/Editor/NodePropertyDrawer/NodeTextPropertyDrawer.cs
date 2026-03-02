using ElfSoft.Framework.Editor;
using ElfSoft.Framework.Editor.UIElements;
using UnityEditor;
using UnityEngine.UIElements;

namespace ElfSoft.DialogueSystem.Editor
{
    internal class NodeTextPropertyDrawer : NodeActionPropertyDrawer
    {
        public LocalTextField TextField { get; private set; }
        public new NodeText Node => base.Node as Node as NodeText;


        public NodeTextPropertyDrawer()
        {
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
                if (e.newValue != e.previousValue) UpdateTextField();
            });
        }

        protected override void UpdateView()
        {
            base.UpdateView();
            UpdateTextField();
        }

        private void UpdateTextField() => TextField.BindString(Node.Text);
    }
}
