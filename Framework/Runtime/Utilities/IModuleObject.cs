using UnityEngine;

namespace ElfSoft.Framework
{
    public interface IModuleObject<T>
    {
#pragma warning disable IDE1006 // 命名样式
        public GameObject gameObject { get; }
        public T module { get; }
#pragma warning restore IDE1006 // 命名样式

    }
}
