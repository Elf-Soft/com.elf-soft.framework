using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ElfSoft.Framework.Editor.UIElements
{
    [CustomPropertyDrawer(typeof(LocalTextAttribute))]
    public class LocalTextAttributeDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            if (property.propertyType == SerializedPropertyType.String)
            {
                
                LocalTextField field = new();
                var attr = attribute as LocalTextAttribute;
                if (attr.MinHeight != default) field.TextElementMinHeight = attr.MinHeight;
                if (attr.MaxHeight != default) field.TextElementMaxHeight = attr.MaxHeight;
                field.BindProperty(property);
                return field;
            }
            else
            {
                PropertyField field = new();
                field.BindProperty(property);
                return field;
            }
        }
    }
}
