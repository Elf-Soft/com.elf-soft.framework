using ElfSoft.Framework;
using ElfSoft.Framework.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElfSoft.DialogueSystem.Editor
{
    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        private readonly Label idLabel;
        public Port Input { get; private set; }
        public Node Node { get; private set; }
        public DialogueDataEditorWindowView EditorView => userData as DialogueDataEditorWindowView;


        public NodeView() : base(AssetDatabase.GetAssetPath(Resources.Load<VisualTreeAsset>("UI/UIDocument/NodeView")))
        {
            idLabel = this.Q<Label>("id-label");

            //创建输入端口
            Input = InstantiatePort(Orientation.Horizontal, Direction.Input, Port.Capacity.Multi, typeof(bool));
            Input.portName = string.Empty;
            inputContainer.Add(Input);

            RegisterCallback<AttachToPanelEvent>(e => EventHub.AddListener<EventData<Node>>(OnNodeEdited));
            RegisterCallback<DetachFromPanelEvent>(e => EventHub.RemoveListener<EventData<Node>>(OnNodeEdited));
        }

        public virtual void BindNode(Node node)
        {
            Node = node;
            title = node.GetType().Name.Replace(nameof(Node), string.Empty);
            var position = ReflectionEx.GetFieldValue<Node, Vector2>(Node, FieldName.position);
            style.left = position.x;
            style.top = position.y;
            UpdateView();
        }

        public virtual void UpdateView()
        {
            idLabel.text = Node.Id.ToString();
        }

        public virtual void UpdateConnect() { }

        public virtual void BeforeElementDeleted()
        {
            EditorView.Asset.DeleteNode(Node);
            outputContainer.Query<Port>().ForEach(port => port.ClearEdges());
        }

        public virtual void BeforeEdgeCreate(Edge edge) { }

        public virtual void BeforeEdgeDelete(Edge edge) { }

        protected virtual void OnNodeEdited(EventData<Node> data)
        {
            if (data.Args == Node && data.Sender != this)
            {
                UpdateView();
                UpdateConnect();
            }
        }

        public virtual VisualElement GetPropertyDrawer()
        {
            var v = new NodePropertyDrawer { userData = userData };
            v.BindNode(Node);
            return v;
        }
    }
}
