using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;

namespace ElfSoft.InventorySystem.Editor
{
    [CustomPropertyDrawer(typeof(ItemSlot), true)]
    public class ItemSlotPropertyDrawer : PropertyDrawer
    {
        private class ItemSlotField : BaseField<ItemSlot>
        {
            private readonly ItemField itemField;
            private readonly IntegerField stackField;
            private const string itemFieldName = "item";
            private const string stackFieldName = "stack";

            public ItemSlotField() : base(nameof(ItemSlot), new())
            {
                AddToClassList(alignedFieldUssClassName);
                var inputElem = this.Q(classes: inputUssClassName);
                inputElem.style.flexDirection = FlexDirection.Row;
                itemField = new();
                itemField.style.flexGrow = 1;
                itemField.labelElement.style.display = DisplayStyle.None;
                itemField.ClearClassList();
                inputElem.Add(itemField);
                stackField = new();
                stackField.style.marginLeft = 4;
                stackField.style.width = 48;
                stackField.ClearClassList();
                inputElem.Add(stackField);

                RegisterCallback<DetachFromPanelEvent>(_ => Unbind());
            }

            public void BindProperty(SerializedProperty property)
            {
                label = property.displayName;
                itemField.BindProperty(property.FindPropertyRelative(itemFieldName));
                stackField.BindProperty(property.FindPropertyRelative(stackFieldName));
            }
            public void Unbind()
            {
                label = string.Empty;
                itemField.Unbind();
                stackField.Unbind();
            }
        }

        public override VisualElement CreatePropertyGUI(SerializedProperty property)
        {
            ItemSlotField field = new();
            field.BindProperty(property);
            return field;
        }
    }
}
