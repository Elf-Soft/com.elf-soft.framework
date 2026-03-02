namespace ElfSoft.InventorySystem
{
    [System.Flags]
    public enum ItemType
    {
        None = 0,

        RegularItem = 1 << 0, KeyItem = 1 << 1,

        Consumable = 1 << 100, Potion = 1 << 101, Food = 1 << 102,

        Equipment = 1 << 200, Shield = 1 << 201, Head = 1 << 202, Body = 1 << 203, Foot = 1 << 204, Accessory = 1 << 205,

        Weapon = 1 << 300, Dagger = 1 << 301, Sword = 1 << 302, Flail = 1 << 303, Axe = 1 << 304, Whip = 1 << 305,
        Cane = 1 << 306, Bow = 1 << 307, Crossbow = 1 << 308, Gun = 1 << 309, Claw = 1 << 310, Glove = 1 << 311, Spear = 1 << 312
    }

    /// <summary>
    /// 稀有度
    /// </summary>
    public enum Rarity
    {
        Trash = -1, N, R, SR, SSR, UR,
    }
}
