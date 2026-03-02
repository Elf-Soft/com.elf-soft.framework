using ElfSoft.Framework;
using UnityEngine.UIElements;

namespace ElfSoft.InventorySystem
{
    [UxmlElement]
    public partial class ItemInfoPanel : VisualElement
    {
        private readonly TextElement descriptionTextElement;
        private static readonly string ussClassName = "item-info-panel";
        private static readonly string textElementUssClassName = ussClassName + "__text-element";
        public string Description
        {
            get => descriptionTextElement.text;
            private set => descriptionTextElement.text = value;
        }


        public ItemInfoPanel()
        {
            UIElementEx.CloneTreeAsset(this, "UI/UIDocument/ItemInfoPanel.uxml");
            AddToClassList(UssClassName.panel);
            AddToClassList(ussClassName);

            descriptionTextElement = this.Q<TextElement>(classes: textElementUssClassName);

            EventHub.AddListener<ItemSlotBarSelectedEventData>(d =>
            {
                var b = d.ItemSlotBar == null;
                Description = b ? string.Empty : d.ItemSlotBar.Slot.Item.Info.Description;
                style.visibility = b ? Visibility.Hidden : Visibility.Visible;
            });
        }

    }
}
