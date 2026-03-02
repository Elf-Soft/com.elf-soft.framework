using ElfSoft.Framework;

namespace ElfSoft.InventorySystem
{
    public abstract class Usable
    {
        public virtual void OnUse(IModuleObject<Inventory<ItemSlot>> obj) { }

    }
}