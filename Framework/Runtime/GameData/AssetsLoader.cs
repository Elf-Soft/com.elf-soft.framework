using System;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace ElfSoft.Framework
{
    public class AssetsLoader<TData, TEntry> where TData : IGameData<TEntry> where TEntry : IGameDataEnty
    {
        private readonly SortedList<int, TEntry> entries = new();
        private AsyncOperationHandle<IList<TData>> handle;
        private static readonly string label = "GameData";


        public AssetsLoader()
        {
            if (handle.IsValid()) return;
            handle = Addressables.LoadAssetsAsync<TData>(label, data =>
            {
                foreach (var e in data.Entries)
                {
                    entries.Add(e.Id, e);
                }
            });
            handle.WaitForCompletion();
        }

        public virtual TEntry GetData(int id)
        {
#if UNITY_EDITOR
            if (!handle.IsDone) throw new System.Exception($"初始化未完成");
            else if (!entries.ContainsKey(id)) throw new System.Exception($"不存在Id:{id}");
#endif
            return entries[id];
        }

    }
}
