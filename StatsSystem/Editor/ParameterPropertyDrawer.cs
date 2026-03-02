using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ElfSoft.StatsSystem.Editor
{
    [CustomPropertyDrawer(typeof(Parameter), true)]
    public class ParameterPropertyDrawer : PropertyDrawer
    {
        private class ParameterField : BaseField<Parameter>
        {
            private readonly EnumField typeField;
            private readonly FloatField valueField;
            private const string valueFieldName = "value";
            private const string typeFieldName = "type";

            public ParameterField() : base(nameof(Parameter), new())
            {
                AddToClassList(alignedFieldUssClassName);
                var inputElem = this.Q(classes: inputUssClassName);
                inputElem.style.flexDirection = FlexDirection.Row;
                typeField = new(defaultValue: ParameterType.Hp);
                typeField.style.flexGrow = 1;
                typeField.ClearClassList();
                inputElem.Add(typeField);
                valueField = new();
                valueField.style.flexGrow = 1;
                valueField.style.marginLeft = 4;
                valueField.ClearClassList();
                inputElem.Add(valueField);

                RegisterCallback<DetachFromPanelEvent>(_ => Unbind());
            }

            public void BindProperty(SerializedProperty property)
            {
                label = property.displayName;
                typeField.BindProperty(property.FindPropertyRelative(typeFieldName));
                valueField.BindProperty(property.FindPropertyRelative(valueFieldName));
            }
            public void Unbind()
            {
                label = string.Empty;
                typeField.Unbind();
                valueField.Unbind();
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ParameterField field = new();
            field.BindProperty(property);
            return field;
        }

    }
}
