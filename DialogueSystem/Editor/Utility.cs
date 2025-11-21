using ElfSoft.Framework.Editor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace ElfSoft.DialogueSystem.Editor
{
    internal static class Utility
    {
        private static readonly Dictionary<Type, Type> ViewTypeCache;


        #region UI
        static Utility()
        {
            ViewTypeCache = new();
            var viewTypes = typeof(NodeView).GetSubTypes();
            foreach (var viewType in viewTypes)
            {
                var att = viewType.GetCustomAttribute<NodeViewAttribute>();
                if (att != null) ViewTypeCache[att.NodeType] = viewType;
            }
        }

        /// <summary>
        /// 根据自定义属性(NodeViewAttribute)获取指定类型视图元素
        /// </summary>
        public static NodeView GetNodeView(Node node)
        {
            var nodeType = node.GetType();
            var type = typeof(NodeView);
            foreach (var key in ViewTypeCache.Keys)
            {
                if (key.IsAssignableFrom(nodeType)) type = ViewTypeCache[key];
            }
            return Activator.CreateInstance(type) as NodeView;
        }

        public static void ClearEdges(this Port port)
        {
            if (port.connections.Count() <= 0) return;
            var list = port.connections.ToList();
            for (int i = list.Count - 1; i >= 0; i--)
            {
                var edge = list[i];
                edge.input.Disconnect(edge);
                edge.output.Disconnect(edge);
                edge.RemoveFromHierarchy();
            }
        }

        #endregion

        #region DialogueData
        public static Node CreateNode(this DialogueData data, Type type)
        {
            var node = Activator.CreateInstance(type) as Node;
            var list = data.Nodes as List<Node>;
            list.Sort((x, y) => x.Id.CompareTo(y.Id));
            var id = list.Count > 0 ? list[^1].Id + 1 : 0;
            ReflectionEx.SetFieldValue(node, FieldName.id, id);
            Undo.RecordObject(data, $"Create node ({data.name})");
            list.Add(node);
            return node;
        }

        public static void DeleteNode(this DialogueData data, Node node)
        {
            Undo.RecordObject(data, $"Delete node ({data.name})");
            (data.Nodes as List<Node>).Remove(node);
        }

        public static void SetNodeId(this DialogueData data, Node node, int value)
        {
            Undo.RecordObject(data, $"Set node id to ({value})");
            ReflectionEx.SetFieldValue(node, FieldName.id, value);
        }

        public static void SetNodeNext(this DialogueData data, NodeAction node, int value)
        {
            Undo.RecordObject(data, $"Set node next to ({value})");
            ReflectionEx.SetFieldValue(node, FieldName.next, value);
        }

        public static void SetOptionNext(this DialogueData data, Option option, int value)
        {
            Undo.RecordObject(data, $"Set option next to ({value})");
            ReflectionEx.SetFieldValue(option, FieldName.next, value);
        }

        public static void SetNodePosition(this DialogueData data, Node node, Vector2 value)
        {
            Undo.RecordObject(data, $"Set position to ({value})");
            ReflectionEx.SetFieldValue(node, FieldName.position, value, ReflectionEx.PrivateFlags);
        }

        #endregion

    }

    internal static class FieldName
    {
        public static readonly string id = "id";
        public static readonly string position = "position";
        public static readonly string next = "next";
        public static readonly string text = "text";
        public static readonly string error = "error";
    }
}
