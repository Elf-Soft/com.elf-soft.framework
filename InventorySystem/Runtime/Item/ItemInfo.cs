using ElfSoft.Framework;
using System;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ElfSoft.InventorySystem
{
    [Serializable]
    public class ItemInfo
    {
        [SerializeField] private int id;
        [SerializeField] private int maxStack;
        [SerializeField] private int price;
        [SerializeField] private Rarity rarity;
        [SerializeField, LocalText] private string name;
        [SerializeField, LocalText(60, 120)] private string description;
        [SerializeField] private AssetReferenceSprite icon;
        //[SerializeField] private Usable useable;
        [SerializeField] private AssetReferenceGameObject prefab;
        public int Id => id;
        public int MaxStack => maxStack;
        public int Price => price;
        public Rarity Rarity => rarity;
        public string Name => name;
        public string Description => description;
        public AssetReferenceSprite Icon => icon;
        public AssetReferenceGameObject Prefab => prefab;
        //public Usable Useable
        //{
        //    get => useable;
        //    private set => useable = value;
        //}
    }

    /// <summary>
    /// Ï¡ÓÐ¶È
    /// </summary>
    public enum Rarity
    {
        Trash = -1, N, R, SR, SSR, UR,
    }
}
