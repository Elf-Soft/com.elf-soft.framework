using ElfSoft.Framework;
using System;
using UnityEngine;

namespace ElfSoft.InventorySystem
{
    [Serializable]
    public class InventorySlot
    {
        [SerializeField] protected Item item;
        [SerializeField] protected int stack;
        public Item Item => item;
        public int Stack => stack;


        /// <summary>
        /// 设置物品栏数据
        /// </summary>
        public void Set(Item _item, int amount)
        {
            if (_item != null && amount > 0)
            {
                ChangeField(_item, amount);
            }
            else Clear();
        }

        /// <summary>
        /// 清空物品栏
        /// </summary>
        public void Clear() => ChangeField(null, 0);

        /// <summary>
        /// 更新数据
        /// </summary>
        protected virtual void ChangeField(Item item, int amount)
        {
            var prviousItem = item;
            var previousStack = stack;
            this.item = item;
            stack = amount;
            EventHub.SendEvent<SlotEvent>(a => a.Init(this, prviousItem, previousStack, item, amount));
        }

        //public bool Swap(Slot slot)
        //{
        //    if (!CheckAllowItem(slot.Item) || !slot.CheckAllowItem(Item)) return false;
        //    if (Item == null || slot.Item == null || Item != slot.Item) return Switch();
        //    int amount = GetCapacity(slot.Item, slot.stack);
        //    Set(slot.Item, stack + amount);
        //    slot.Set(slot.Item, slot.stack - amount);
        //    return true;

        //    bool Switch()
        //    {
        //        var item = Item;
        //        var amount = stack;
        //        Set(slot.Item, slot.stack);
        //        slot.Set(item, amount);
        //        return true;
        //    }
        //}

        /// <summary>
        /// 传入[增加数量],返回实际可增加数
        /// </summary>
        public int GetCapacity(Item _item, int toAdd)
        {
            var canAdd = 0;
            if (Allow(_item))
            {
                canAdd = _item.Info.MaxStack - Stack;
                canAdd = canAdd <= toAdd ? canAdd : toAdd;
            }
            return canAdd;
        }

        /// <summary>
        /// 判断插槽是否可以放入对应物品
        /// </summary>
        public virtual bool Allow(Item _item)
        {
            return Item == null || Item.Id == _item.Id;
        }

    }
}
