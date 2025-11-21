using System;

namespace ElfSoft.Framework.Editor
{
    public readonly struct TypeMenuData
    {
        public readonly Type type;
        public readonly string path;
        public readonly string name;

        public TypeMenuData(Type type, string path, string name)
        {
            this.type = type;
            this.path = string.IsNullOrEmpty(path) ? string.Empty : path;
            this.name = string.IsNullOrEmpty(name) ? type.Name : name;
        }
    }
}
