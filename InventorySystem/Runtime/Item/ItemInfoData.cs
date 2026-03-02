using ElfSoft.Framework;
using System.Collections.Generic;
using UnityEngine;

namespace ElfSoft.InventorySystem
{
    [CreateAssetMenu(menuName = "GameData/Inventory System/ItemInfoData", fileName = "ItemInfoData")]
    public class ItemInfoData : ScriptableObject, IGameData<ItemInfo>
    {
        [SerializeField] private List<ItemInfo> entries;
        public IReadOnlyList<ItemInfo> Entries => entries;
    }
}
