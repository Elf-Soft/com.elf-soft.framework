using UnityEngine.ResourceManagement.AsyncOperations;

namespace ElfSoft.Framework
{
    public static class AddressablesEx
    {
        /// <summary>
        /// 如果加载句柄有效则释放
        /// </summary>
        public static void TryRelease<T>(this AsyncOperationHandle<T> handel)
        {
            if (handel.IsValid()) handel.Release();
        }

    }
}
