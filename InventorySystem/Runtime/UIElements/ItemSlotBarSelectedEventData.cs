using ElfSoft.Framework;

namespace ElfSoft.InventorySystem
{
    public class ItemSlotBarSelectedEventData : EventData
    {
        public ItemSlotBar ItemSlotBar { get; private set; }

        public void Init(ItemSlotBar bar) => ItemSlotBar = bar;

        protected override void Reset()
        {
            ItemSlotBar = null;
        }
    }
}
