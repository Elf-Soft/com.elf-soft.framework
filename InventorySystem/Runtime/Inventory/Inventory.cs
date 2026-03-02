using ElfSoft.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ElfSoft.InventorySystem
{
    [Serializable]
    public class Inventory<T> where T : ItemSlot, new()
    {
        [SerializeField] private List<T> slots = new();
        public IReadOnlyList<T> Slots => slots;


        /// <summary>
        /// 向库存中添加物品
        /// </summary>
        public void AddItem(Item item, int toAdd)
        {
            foreach (var slot in slots)
            {
                int add = slot.GetCapacity(item, toAdd);
                if (add > 0)
                {
                    slot.Set(item, slot.Stack + add);
                    toAdd -= add;
                    if (toAdd <= 0) break;
                }
            }
            EventHub.SendEvent<InventoryChangedEventData<T>>(e => e.Init(this));
        }

        /// <summary>
        /// 从库存中移除指定类型物品
        /// </summary>
        public void RemoveItem(int id, int toRemove)
        {
            foreach (var slot in slots)
            {
                if (slot.Item != null && slot.Item.Id == id)
                {
                    var remove = slot.Stack > toRemove ? toRemove : slot.Stack;
                    if (remove > 0)
                    {
                        var item = slot.Item;
                        slot.Set(item, slot.Stack - remove);
                        toRemove -= remove;
                        if (toRemove <= 0) break;
                    }
                }
            }
            EventHub.SendEvent<InventoryChangedEventData<T>>(e => e.Init(this));
        }

        /// <summary>
        /// 获取对应物品可以容纳数量
        /// </summary>
        public int GetCapacity(Item item)
        {
            int canAdd = 0;
            foreach (var slot in slots)
            {
                canAdd += slot.GetCapacity(item, item.Info.MaxStack);
            }
            return canAdd;
        }

        /// <summary>
        /// 查找对应物品在库存中的总数
        /// </summary>
        public int GetAmount(Item item)
        {
            int amount = 0;
            foreach (var slot in slots)
            {
                if (slot.Item.Id == item.Id) amount += slot.Stack;
            }
            return amount;
        }

        /// <summary>
        /// 查找非空栏位数量
        /// </summary>
        public int GetNonEmptySlotAmount()
        {
            int amount = 0;
            foreach (var slot in slots)
            {
                if (slot.Item != null) amount++;
            }
            return amount;
        }
    }
}
