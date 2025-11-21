using ElfSoft.Framework;

namespace ElfSoft.InventorySystem
{
    public class InventoryEvent : EventData
    {
        public IInventory Target { get; protected set; }


        public void Init(IInventory target)
        {
            Target = target;
        }

        protected override void Reset()
        {
            Target = null;
        }
    }
}
