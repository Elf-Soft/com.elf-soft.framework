using ElfSoft.Framework;

namespace ElfSoft.InventorySystem
{
    public class SlotEvent : EventData
    {
        public InventorySlot Target { get; private set; }
        public Item PreviousItem { get; private set; }
        public Item NewItem { get; private set; }
        public int PreviousStack { get; private set; }
        public int NewStack { get; private set; }


        public void Init(InventorySlot target, Item prviousItem, int previousStack, Item newItem, int newStack)
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
