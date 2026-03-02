using ElfSoft.Framework;

namespace ElfSoft.InventorySystem
{
    public class ItemInfoDataBase
    {
        private readonly AssetsLoader<ItemInfoData, ItemInfo> loader = new();
        public static ItemInfoDataBase Instance { get; } = new();


        public ItemInfo GetItemInfo(int id)
        {
            return loader.GetData(id);
        }

    }
}
