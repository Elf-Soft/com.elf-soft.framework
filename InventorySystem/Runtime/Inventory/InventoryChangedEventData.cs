using ElfSoft.Framework;

namespace ElfSoft.InventorySystem
{
    public class InventoryChangedEventData<T> : EventData where T : ItemSlot, new()
    {
        public Inventory<T> Target { get; protected set; }


        public void Init(Inventory<T> target)
        {
            Target = target;
        }

        protected override void Reset()
        {
            Target = null;
        }
    }
}
