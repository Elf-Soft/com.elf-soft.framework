using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace ElfSoft.Framework.Editor.UIElements
{
    public class GameDataEntryBar : VisualElement
    {
        public Label IdLabel { get; private set; }
        public Label NameLabel { get; private set; }

        public GameDataEntryBar()
        {
            var tree = Resources.Load<VisualTreeAsset>("UI/UIDocument/GameDataEntryBar");
            tree.CloneTree(this);

            IdLabel = this.Q<Label>("id-label");
            NameLabel = this.Q<Label>("name-label");
        }

        public void BindProperty(SerializedProperty idProperty, SerializedProperty nameProperty)
        {
            IdLabel.BindProperty(idProperty);
            NameLabel.BindProperty(nameProperty);
        }

        public void UnBind()
        {
            IdLabel.Unbind();
            NameLabel.Unbind();
        }
    }
}
