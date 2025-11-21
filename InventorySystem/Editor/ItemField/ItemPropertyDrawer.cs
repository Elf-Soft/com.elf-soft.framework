using UnityEditor;
using UnityEngine.UIElements;

namespace ElfSoft.InventorySystem.Editor
{
    [CustomPropertyDrawer(typeof(Item), true)]
    public class ItemPropertyDrawer : PropertyDrawer
    {
        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ItemField field = new();
            field.BindProperty(property);
            return field;
        }
    }
}
