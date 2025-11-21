using System;

namespace ElfSoft.Framework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface)]
    public class TypeMenuAttribute : Attribute
    {
        public readonly int Order;
        public readonly string Path;
        public readonly string Name;

        public TypeMenuAttribute(int order, string path = null, string name = null)
        {
            Order = order;
            Path = path ?? string.Empty;
            Name = name ?? string.Empty;
        }
    }
}
