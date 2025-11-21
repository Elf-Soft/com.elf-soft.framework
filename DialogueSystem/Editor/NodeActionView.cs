using ElfSoft.Framework;
using ElfSoft.Framework.Editor;
using System.Reflection.Emit;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElfSoft.DialogueSystem.Editor
{
    [NodeView(typeof(NodeAction))]
    public class NodeActionView : NodeView
    {
        public Port Output { get; private set; }
        protected virtual string StyleClass => "action";
        public new NodeAction Node => base.Node as NodeAction;


        public NodeActionView()
        {
            Output = InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool));
            Output.portName = nameof(NodeAction.Next);
            outputContainer.Add(Output);

            AddToClassList(StyleClass);
        }

        public override void UpdateConnect()
        {
            Output.ClearEdges();
            if (Node.Next != -1) EditorView.GraphView.CreateEdge(Output, Node.Next);
        }

        public override void BeforeEdgeCreate(Edge edge)
        {
            var v = edge.input.node as NodeView;
            EditorView.Asset.SetNodeNext(Node, v.Node.Id);
            EventHub.SendEvent<EventData<Node>>(e => e.Init(this, Node));
        }

        public override void BeforeEdgeDelete(Edge edge)
        {
            EditorView.Asset.SetNodeNext(Node, -1);
            EventHub.SendEvent<EventData<Node>>(e => e.Init(this, Node));
        }

        public override VisualElement GetPropertyDrawer()
        {
            var v = new NodeActionPropertyDrawer { userData = userData };
            v.BindNode(Node);
            return v;
        }
    }
}
