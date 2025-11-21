using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ElfSoft.Framework.Editor.UIElements
{
    public class CustomEditor<T> : UnityEditor.Editor where T : EditorWindow
    {
        public override VisualElement CreateInspectorGUI()
        {
            VisualElement root = new();

            PropertyField scriptField = new();
            scriptField.BindProperty(serializedObject.FindProperty("m_Script"));
            scriptField.SetEnabled(false);
            root.Add(scriptField);

            Button button = CreateButton();
            button.style.marginTop = 10;
            button.style.minHeight = 36;
            root.Add(button);
            return root;
        }

        protected virtual Button CreateButton()
        {
            return new(() => EditorWindow.GetWindow<T>()) { text = "Editor in Project Setting Window" };
        }
    }
}
