using ElfSoft.Framework.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElfSoft.DialogueSystem.Editor
{
    public class DialogueGraphView : GraphView
    {
        private Vector2 pointerPosition;
        private readonly HashSet<GraphElement> copyCache = new();
        private readonly List<(string actionName, Action<DropdownMenuAction> action)> menuActionCaches = new();
        public DialogueDataEditorWindowView EditorView { get; private set; }
        public int SelectedIndex { get; private set; } = -1;


        public DialogueGraphView(DialogueDataEditorWindowView root)
        {
            EditorView = root;
            //添加网格视图
            Insert(0, new GridBackground());

            //添加操纵器
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale * 2f);
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());

            //缓存指针位置
            RegisterCallback<PointerMoveEvent>(evt => pointerPosition = evt.position);

            //复制
            serializeGraphElements += elements =>
            {
                copyCache.Clear();
                foreach (var e in elements)
                {
                    copyCache.Add(e);
                }
                return null;
            };

            //粘贴
            canPasteSerializedData += data => copyCache.Count > 0;
            unserializeAndPaste += (operationName, data) =>
            {
                Vector2 position = pointerPosition;
                foreach (var graphElement in copyCache)
                {
                    if (graphElement is NodeView view)
                    {
                        var node = EditorView.Asset.CreateNode(view.Node.GetType());
                        var newView = CreateNodeView(node);
                        var local = contentViewContainer.WorldToLocal(position);
                        //将坐标限制在视图内
                        var xMax = worldBound.xMax - position.x;
                        local.x = xMax > view.resolvedStyle.width ? local.x : local.x - view.resolvedStyle.width;
                        var yMax = worldBound.yMax - position.y;
                        local.y = yMax > view.resolvedStyle.height ? local.y : local.y - view.resolvedStyle.height;
                        //设置位置
                        newView.SetPosition(new Rect(local, Vector2.zero));
                        position.x += view.resolvedStyle.width;
                    }
                }
            };

            //初始化菜单项动作缓存
            InitMenuActionCaches();

            //加载样式
            var ss = Resources.Load<StyleSheet>("UI/StyleSheet/DialogueGraphViewStyle");
            styleSheets.Add(ss);
        }

        public void UpdateView()
        {
            graphViewChanged -= OnGrahpViewChanged;
            DeleteElements(graphElements);
            if (EditorView.Asset == null) return;
            graphViewChanged += OnGrahpViewChanged;

            //初始化节点元素
            var nodes = EditorView.Asset.Nodes;
            for (int i = 0; i < nodes.Count; i++)
            {
                CreateNodeView(nodes[i]);
            }
            foreach (var view in this.nodes)
            {
                var v = view as NodeView;
                v.UpdateConnect();
                if (v.Node.Id == SelectedIndex) AddToSelection(view);
            }
        }

        //视图元素修改时
        private GraphViewChange OnGrahpViewChanged(GraphViewChange change)
        {
            change.movedElements?.ForEach(elem =>
            {
                if (elem is not NodeView v) return;
                var rect = v.GetPosition();
                EditorView.Asset.SetNodePosition(v.Node, new Vector2(rect.x, rect.y));
            });

            change.elementsToRemove?.ForEach(elem =>
            {
                //删除节点元素时
                if (elem is NodeView nodeView)
                {
                    nodeView.BeforeElementDeleted();
                    //如果是选中项则更新属性面板
                    if (nodeView.Node.Id == SelectedIndex) EditorView.PropertiesPanel.Clear();
                }
                //删除连接线段时
                else if (elem is Edge edge && edge.output.node is NodeView v)
                {
                    v.BeforeEdgeDelete(edge);
                }
            });

            //创建连接线段时,设置对应节点选项
            change.edgesToCreate?.ForEach(edge =>
            {
                if (edge.output.node is NodeView view)
                {
                    edge.output.RemoveFromClassList(FieldName.error);
                    view.BeforeEdgeCreate(edge);
                }
            });

            EditorView.So.ApplyModifiedProperties();
            return change;
        }

        private NodeView CreateNodeView(Node node)
        {
            NodeView v = Utility.GetNodeView(node);
            v.userData = EditorView;
            v.BindNode(node);
            AddElement(v);
            return v;
        }

        /// <summary>
        /// 创建连线
        /// </summary>
        public void CreateEdge(Port port, int next)
        {
            var views = contentContainer.Query<NodeView>().ToList();
            var nextView = views.Find(v => next == v.Node.Id);
            if (nextView != null)
            {
                var edge = port.ConnectTo(nextView.Input);
                AddElement(edge);
                port.RemoveFromClassList(FieldName.error);
            }
            else port.AddToClassList(FieldName.error);
        }


        /// <summary>
        /// 重写获取兼容端口
        /// </summary>
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList().Where(endPort =>
                endPort.direction != startPort.direction &&
                endPort.node != startPort.node).ToList();
        }

        /// <summary>
        /// 重写选择元素时
        /// </summary>
        public override void AddToSelection(ISelectable selectable)
        {
            base.AddToSelection(selectable);
            if (selectable is not NodeView v) return;
            SelectedIndex = v.Node.Id;
            EditorView.RefreshProperties(v.GetPropertyDrawer());
        }

        /// <summary>
        /// 重写构建菜单项方法
        /// </summary>
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            //创建节点
            foreach (var (actionName, action) in menuActionCaches)
            {
                evt.menu.AppendAction(actionName, action, EditorView.CheckAsset);
            }

            //创建基础菜单项
            evt.menu.AppendSeparator();
            base.BuildContextualMenu(evt);

            //本编辑器中禁用duplicate功能，从菜单中删除
            var dulplicateIndex = evt.menu.MenuItems().Count - 2;
            if (dulplicateIndex >= 0) evt.menu.RemoveItemAt(dulplicateIndex);
        }

        /// <summary>
        /// 构建菜单项缓存:据类型所有子类创建节点
        /// </summary>
        private void InitMenuActionCaches()
        {
            foreach (var d in TypeMenuUtility.GetSubTypeTypeMenuDatas(typeof(Node)))
            {
                (string actionName, Action<DropdownMenuAction> action) actionCache = new()
                {
                    actionName = d.path + d.name.Replace(nameof(Node), string.Empty),
                    action = a =>
                    {
                        var position = contentViewContainer.WorldToLocal(a.eventInfo.localMousePosition);
                        var node = EditorView.Asset.CreateNode(d.type);
                        EditorView.Asset.SetNodePosition(node, position);
                        var nodeView = CreateNodeView(node);
                    }
                };
                menuActionCaches.Add(actionCache);
            }
        }

    }
}
