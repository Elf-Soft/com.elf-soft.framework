using UnityEngine.UIElements;

namespace ElfSoft.DialogueSystem.Editor
{
    [NodeView(typeof(NodeText))]
    public class NodeTextView : NodeActionView
    {
        protected override string StyleClass => "text";

        public override VisualElement GetPropertyDrawer()
        {
            var v = new NodeTextPropertyDrawer { userData = userData };
            v.BindNode(Node);
            return v;
        }
    }
}
