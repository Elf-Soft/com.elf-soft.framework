using System;

namespace ElfSoft.DialogueSystem.Editor
{
    [AttributeUsage(AttributeTargets.Class)]
    public class NodeViewAttribute : Attribute
    {
        public Type NodeType;
        public string[] Styles;

        public NodeViewAttribute(Type type, params string[] styles)
        {
            NodeType = type;
            Styles = styles;
        }
    }
}
