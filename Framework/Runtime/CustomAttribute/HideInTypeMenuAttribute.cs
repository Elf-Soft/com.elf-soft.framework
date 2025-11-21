using System;

namespace ElfSoft.Framework
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Enum | AttributeTargets.Interface, AllowMultiple = false)]
    public class HideInTypeMenuAttribute : Attribute { }
}
