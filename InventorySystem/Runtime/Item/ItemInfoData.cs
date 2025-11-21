using System.Collections.Generic;
using UnityEngine;

namespace ElfSoft.InventorySystem
{
    [CreateAssetMenu(menuName = "GameData/Inventory System/ItemInfoData", fileName = "ItemInfoData")]
    public class ItemInfoData : ScriptableObject
    {
        [SerializeField] private List<ItemInfo> infos;
        public IReadOnlyList<ItemInfo> Infos => infos;

    }
}
