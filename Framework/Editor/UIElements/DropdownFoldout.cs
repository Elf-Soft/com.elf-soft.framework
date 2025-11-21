using UnityEditor;
using UnityEngine.UI;
using UnityEngine.UIElements;

namespace ElfSoft.Framework.Editor.UIElements
{
    public class DropdownFoldout : VisualElement
    {
        protected SerializedProperty property;
        public Foldout Foldout { get; private set; }
        public DropdownField Dropdown { get; private set; }
        public string Title
        {
            get => Foldout.text;
            set => Foldout.text = value;
        }


        public DropdownFoldout()
        {
            Dropdown = new();
            Dropdown.style.flexGrow = 1;
            Dropdown.style.marginRight = 0;

            Foldout = new() { text = nameof(Foldout) };
            var label = Foldout.Q<Label>();
            label.style.flexGrow = 0;
            label.style.minWidth = 103;
            Foldout.Q<VisualElement>(null, "unity-toggle__input").Add(Dropdown);
            Add(Foldout);

            Foldout.RegisterValueChangedCallback(e =>
            {
                if (property != null) property.isExpanded = e.newValue;
            });

        }

        public virtual void BindProperty(SerializedProperty property)
        {
            this.property = property;
            Title = property.displayName;
            Foldout.SetValueWithoutNotify(property.isExpanded);
        }
    }
}
