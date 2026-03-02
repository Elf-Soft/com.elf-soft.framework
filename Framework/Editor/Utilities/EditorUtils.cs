using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ElfSoft.Framework.Editor
{
    public static class EditorUtils
    {
        public static void RegisterUndoEvent(this VisualElement element, Undo.UndoRedoCallback action)
        {
            element.RegisterCallback<AttachToPanelEvent>(e => Undo.undoRedoPerformed += action);
            element.RegisterCallback<DetachFromPanelEvent>(e => Undo.undoRedoPerformed -= action);
        }

        /// <summary>
        /// 在UI元素下显示目标序列化属性的所有序列化子属性
        /// </summary>
        public static void ShowProperties(VisualElement content, SerializedProperty property, Func<SerializedProperty, bool> check = null)
        {
            content.Clear();
            if (property == null) return;
            foreach (var p in property.GetSubProperties())
            {
                if (check != null && !check(p)) continue;
                PropertyField propertyField = new();
                propertyField.BindProperty(p);
                content.Add(propertyField);
            }
        }

        /// <summary>
        /// 获取序列化属性的所有子属性
        /// </summary>
        public static IEnumerable<SerializedProperty> GetSubProperties(this SerializedProperty property)
        {
            int depth = property.depth + 1;
            var enumerator = property.GetEnumerator();

            while (enumerator.MoveNext())
            {
                if (enumerator.Current is not SerializedProperty child) continue;
                if (child.depth > depth) continue;
                yield return child;
            }
        }

        /// <summary>
        /// 向序列化对象的集合属性添加元素
        /// </summary>
        public static SerializedProperty AddArrayElement(this SerializedObject so, string propertyName, Action<SerializedProperty> callback = null)
        {
            var property = so.FindProperty(propertyName);
            property.InsertArrayElementAtIndex(property.arraySize);
            var p = property.GetArrayElementAtIndex(property.arraySize - 1);
            callback?.Invoke(p);
            so.ApplyModifiedProperties();
            return p;
        }

        /// <summary>
        /// 从序列化对象的集合属性删除元素
        /// </summary>
        public static void DeleteArrayElementAtIndex(this SerializedObject so, string propertyName, int index)
        {
            var p = so.FindProperty(propertyName);
            p.DeleteArrayElementAtIndex(index);
            so.ApplyModifiedProperties();
        }

        public static SerializedProperty AddArrayReferenceElement(this SerializedObject so, string propertyName, Type type, Action<SerializedProperty> callback = null)
        {
            var p = AddArrayElement(so, propertyName, p => p.managedReferenceValue = Activator.CreateInstance(type));
            callback?.Invoke(p);
            so.ApplyModifiedProperties();
            return p;
        }


        /// <summary>
        /// 创建撤销组合
        /// </summary>
        public static void CollapseUndo<TObj>(this TObj objectToUndo, ref int undoGroupCache, string undoName, Action action) where TObj : UnityEngine.Object
        {
            if (undoGroupCache != Undo.GetCurrentGroup() && Undo.GetCurrentGroupName() != undoName)
            {
                Undo.SetCurrentGroupName(undoName);
                undoGroupCache = Undo.GetCurrentGroup();
            }
            Undo.RecordObject(objectToUndo, undoName);
            action();
            Undo.CollapseUndoOperations(undoGroupCache);
        }

    }
}
