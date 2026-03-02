using ElfSoft.Framework;

namespace ElfSoft.InventorySystem
{
    public class SlotChangedEventData : EventData
    {
        public ItemSlot Target { get; private set; }
        public Item PreviousItem { get; private set; }
        public Item NewItem { get; private set; }
        public int PreviousStack { get; private set; }
        public int NewStack { get; private set; }


        public void Init(ItemSlot target, Item prviousItem, int previousStack, Item newItem, int newStack)
        {
            Target = target;
            PreviousItem = prviousItem;
            PreviousStack = previousStack;
            NewItem = newItem;
            NewStack = newStack;
        }

        protected override void Reset()
        {
            Target = null;
            PreviousItem = null;
            PreviousStack = 0;
            NewItem = null;
            NewStack = 0;
        }
    }
}
