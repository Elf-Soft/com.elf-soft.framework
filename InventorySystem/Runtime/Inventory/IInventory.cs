using System.Collections.Generic;

namespace ElfSoft.InventorySystem
{
    public interface IInventory
    {
        public IReadOnlyList<InventorySlot> Slots { get; }


        /// <summary>
        /// 向库存中添加物品
        /// </summary>
        public void AddItem(Item item, int toAdd);

        /// <summary>
        /// 从库存中移除指定类型物品
        /// </summary>
        public void RemoveItem(int id, int toRemove);

        /// <summary>
        /// 获取对应物品可以容纳数量
        /// </summary>
        public int GetCapacity(Item item);

        /// <summary>
        /// 查找对应物品在库存中的总数
        /// </summary>
        public int GetAmount(Item item);

    }
}
