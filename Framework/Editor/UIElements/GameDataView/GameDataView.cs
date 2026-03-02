using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElfSoft.Framework.Editor.UIElements
{
    public abstract class GameDataView<T> : VisualElement where T : Object
    {
        public ToolbarMenu Menu { get; private set; }
        public ObjectField ObjField { get; private set; }
        public T Asset { get; private set; }
        public SerializedObject So { get; private set; }


        public GameDataView() : this("UI/UIDocument/GameDataView") { }

        public GameDataView(string docPath)
        {
            var tree = Resources.Load<VisualTreeAsset>(docPath);
            tree.CloneTree(this);
            style.flexGrow = 1;

            Menu = this.Q<ToolbarMenu>("menu");
            ObjField = this.Q<ObjectField>();
            ObjField.objectType = typeof(T);
            ObjField.RegisterValueChangedCallback(evt => Show(evt.newValue as T));


            RegisterCallback<AttachToPanelEvent>(evt => Undo.undoRedoPerformed += UpdateView);
            RegisterCallback<DetachFromPanelEvent>(evt =>
            {
                Undo.undoRedoPerformed -= UpdateView;
                Dispose();
            });
        }

        public void Show(T target)
        {
            if (Asset == target) return;
            SwitchAsset(target);
            UpdateView();
        }

        protected virtual void SwitchAsset(T target)
        {
            Asset = target;
            ObjField.SetValueWithoutNotify(target);
            Dispose();
            So = target != null ? new(target) : null;
        }

        protected virtual void Dispose()
        {
            if (Asset == null) return;
            EditorUtility.SetDirty(Asset);
            AssetDatabase.SaveAssetIfDirty(Asset);
            So?.Dispose();
        }

        protected virtual void UpdateView()
        {
            So?.Update();
        }

        public DropdownMenuAction.Status CheckAsset(DropdownMenuAction _)
        {
            return Asset != null ? DropdownMenuAction.Status.Normal : DropdownMenuAction.Status.Disabled;
        }

    }

}
