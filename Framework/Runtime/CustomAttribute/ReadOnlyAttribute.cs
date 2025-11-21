using System;

namespace ElfSoft.Framework
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ReadOnlyAttribute : Attribute { }
}
