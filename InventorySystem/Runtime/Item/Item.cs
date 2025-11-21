using System;
using UnityEngine;

namespace ElfSoft.InventorySystem
{
    [Serializable]
    public class Item
    {
        [SerializeField] private int id;
        public int Id => id;
        public ItemInfo Info => default/*ItemDataManager.Instance.DataDic[Id]*/;
        //public static explicit operator Item(int id) => ItemDataManager.Instance.GetData(id);

    }
}
