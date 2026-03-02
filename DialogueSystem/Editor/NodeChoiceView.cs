using ElfSoft.Framework;
using System.Collections;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

namespace ElfSoft.DialogueSystem.Editor
{
    [NodeView(typeof(NodeChoice))]
    internal class NodeChoiceView : NodeView
    {
        private readonly ListView listView;
        public new NodeChoice Node => base.Node as NodeChoice;


        public NodeChoiceView()
        {
            listView = new ListView()
            {
                virtualizationMethod = CollectionVirtualizationMethod.DynamicHeight,
                makeItem = () => InstantiatePort(Orientation.Horizontal, Direction.Output, Port.Capacity.Single, typeof(bool)),
                bindItem = (elem, i) =>
                {
                    var port = elem as Port;
                    var c = Node.Choices[i];
                    port.userData = c;
                    port.portName = i.ToString();
                    var next = c.Next;
                    if (next != -1) EditorView.GraphView.CreateEdge(port, next);
                },
            };
            outputContainer.Add(listView);

            RegisterCallback<AttachToPanelEvent>(e => EventHub.AddListener<EventData<(Node, Option)>>(OnNodeEdited));
            RegisterCallback<DetachFromPanelEvent>(e => EventHub.RemoveListener<EventData<(Node, Option)>>(OnNodeEdited));

            void OnNodeEdited(EventData<(Node node, Option choice)> e)
            {
                if (e.Args.node == Node && e.Sender != this)
                {
                    UpdateView();
                    UpdateConnect();
                }
            }

            AddToClassList("choice");
        }

        public override void BindNode(Node node)
        {
            base.BindNode(node);
            listView.itemsSource = Node.Choices as IList;
        }

        public override void UpdateConnect()
        {
            outputContainer.Query<Port>().ForEach(port => port.ClearEdges());
            listView.RefreshItems();
        }

        public override void BeforeEdgeCreate(Edge edge)
        {
            var v = edge.input.node as NodeView;
            var o = edge.output.userData as Option;
            EditorView.Asset.SetOptionNext(o, v.Node.Id);
            EventHub.SendEvent<EventData<(Node, Option)>>(e => e.Init(this, (Node, o)));
        }

        public override void BeforeEdgeDelete(Edge edge)
        {
            var o = edge.output.userData as Option;
            EditorView.Asset.SetOptionNext(o, -1);
            EventHub.SendEvent<EventData<(Node, Option)>>(e => e.Init(this, (Node, o)));
        }

        public override VisualElement GetPropertyDrawer()
        {
            var v = new NodeChoicePropertyDrawer { userData = userData };
            v.BindNode(Node);
            return v;
        }
    }
}
