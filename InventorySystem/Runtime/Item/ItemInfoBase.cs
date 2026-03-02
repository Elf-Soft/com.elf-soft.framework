using ElfSoft.Framework;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ElfSoft.InventorySystem
{
    public class ItemInfoBase : IGameDataEnty
    {
        [SerializeField] private int id;
        [SerializeField] private int maxStack;
        [SerializeField] private int price;
        [SerializeField] private ItemType type;
        [SerializeField] private Rarity rarity;
        [SerializeField, LocalText] private string name;
        [SerializeField, LocalText(60, 120)] private string description;
        [SerializeField] private AssetReferenceSprite icon;
        [SerializeField] private AssetReferenceGameObject prefab;
        public int Id => id;
        public int MaxStack => maxStack;
        public int Price => price;
        public ItemType Type => type;
        public Rarity Rarity => rarity;
        public string Name => LocalizationEx.GetLocalizedString(name);
        public string Description => LocalizationEx.GetLocalizedString(description);
        public AssetReferenceSprite Icon => icon;
        public AssetReferenceGameObject Prefab => prefab;
    }
}
